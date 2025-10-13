using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JwtApi.Api.Middleware
{
    public class SecurityOptions
    {
        public bool EnableHeaders { get; set; } = true;
        public string ContentSecurityPolicy { get; set; } = "default-src 'self'";
        public TimeSpan HstsMaxAge { get; set; } = TimeSpan.FromDays(365);
        public bool EnableRateLimiting { get; set; } = true;
        public int RequestsPerWindow { get; set; } = 100; // defaults
        public TimeSpan RateLimitWindow { get; set; } = TimeSpan.FromMinutes(1);
        public long MaxRequestBodyBytes { get; set; } = 10 * 1024 * 1024; // 10 MB
        public string[] BlockedUserAgentsStartsWith { get; set; } = new[] { "curl", "python-requests", "nikto", "sqlmap" };
        public bool EnablePayloadInspection { get; set; } = true;
        public string[] SuspiciousPatterns { get; set; } = new[]
        {
            @"(\%27)|(\')|(\-\-)|(\%23)|(#)", // basic SQL injection tokens
            @"((\%3C)|<).*script.*((\%3E)|>)", // script tag -> XSS
            @"(union(\s)+select)", // SQLi union select
            @"(drop|delete|truncate)\s+table", // destructive SQL
            @"(or(\s)+1=1)" // tautology
        };
        // For token blacklist / revocation
        public bool EnableTokenBlacklist { get; set; } = true;
    }

    public interface ITokenBlacklist
    {
        Task<bool> IsBlacklistedAsync(string token);
        Task BlacklistAsync(string token, TimeSpan? ttl = null);
    }

    // Simple in-memory token blacklist - replace with Redis or DB in production
    public class InMemoryTokenBlacklist : ITokenBlacklist
    {
        private readonly ConcurrentDictionary<string, DateTime> _store = new();
        public Task BlacklistAsync(string token, TimeSpan? ttl = null)
        {
            var expiry = DateTime.UtcNow + (ttl ?? TimeSpan.FromDays(7));
            _store[token] = expiry;
            return Task.CompletedTask;
        }

        public Task<bool> IsBlacklistedAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return Task.FromResult(false);
            if (_store.TryGetValue(token, out var expiry))
            {
                if (expiry < DateTime.UtcNow)
                {
                    _store.TryRemove(token, out _);
                    return Task.FromResult(false);
                }
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }

    public class SecurityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SecurityMiddleware> _logger;
        private readonly SecurityOptions _options;

        // simple sliding window counters: IP -> list of ticks (could be optimized to bucket)
        private static readonly ConcurrentDictionary<string, ConcurrentQueue<long>> _ipRequests = new();
        private readonly List<Regex> _suspiciousRegexes;

        private readonly ITokenBlacklist? _tokenBlacklist;

        public SecurityMiddleware(RequestDelegate next, IOptions<SecurityOptions> options,
            ILogger<SecurityMiddleware> logger, IServiceProvider sp)
        {
            _next = next;
            _logger = logger;
            _options = options.Value;
            _suspiciousRegexes = _options.SuspiciousPatterns.Select(p => new Regex(p, RegexOptions.IgnoreCase | RegexOptions.Compiled)).ToList();
            // optional token blacklist injected if registered
            _tokenBlacklist = sp.GetService(typeof(ITokenBlacklist)) as ITokenBlacklist;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 1) enforce HTTPS
            if (!context.Request.IsHttps)
            {
                // Optionally redirect or reject
                // We'll reject for APIs (client must use HTTPS)
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("HTTPS required.");
                _logger.LogWarning("Rejected non-HTTPS request from {RemoteIp}", context.Connection.RemoteIpAddress);
                return;
            }

            // 2) Security headers
            if (_options.EnableHeaders)
            {
                AddSecurityHeaders(context);
            }

            // 3) Basic UA blocking
            if (context.Request.Headers.TryGetValue("User-Agent", out var ua))
            {
                var uaLower = ua.ToString().ToLowerInvariant();
                if (_options.BlockedUserAgentsStartsWith.Any(prefix => uaLower.StartsWith(prefix)))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Forbidden");
                    _logger.LogWarning("Blocked request due to UA from {Ip}: {UA}", context.Connection.RemoteIpAddress, ua);
                    return;
                }
            }

            // 4) Rate limiting by IP
            if (_options.EnableRateLimiting)
            {
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                if (IsRateLimitExceeded(ip))
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.Response.Headers["Retry-After"] = _options.RateLimitWindow.TotalSeconds.ToString();
                    await context.Response.WriteAsync("Too many requests");
                    _logger.LogWarning("Rate limit exceeded for {Ip}", ip);
                    return;
                }
            }

            // 5) Request size limit
            if (context.Request.ContentLength.HasValue && context.Request.ContentLength.Value > _options.MaxRequestBodyBytes)
            {
                context.Response.StatusCode = StatusCodes.Status413PayloadTooLarge;
                await context.Response.WriteAsync("Payload too large");
                _logger.LogWarning("Payload too large from {Ip} ({Len} bytes)", context.Connection.RemoteIpAddress, context.Request.ContentLength.Value);
                return;
            }

            // 6) Content-Type enforcement for JSON modifying endpoints
            if (HttpMethods.IsPost(context.Request.Method) ||
                HttpMethods.IsPut(context.Request.Method) ||
                HttpMethods.IsPatch(context.Request.Method))
            {
                if (!context.Request.Headers.ContainsKey("Content-Type") ||
                    !context.Request.ContentType!.ToLowerInvariant().Contains("application/json"))
                {
                    context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
                    await context.Response.WriteAsync("Content-Type must be application/json");
                    _logger.LogWarning("Unsupported Content-Type from {Ip}: {CT}", context.Connection.RemoteIpAddress, context.Request.ContentType);
                    return;
                }
            }

            // 7) Quick payload inspection (query string + small body)
            if (_options.EnablePayloadInspection)
            {
                // check query
                var query = context.Request.QueryString.Value ?? string.Empty;
                if (IsSuspicious(query))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Bad request");
                    _logger.LogWarning("Suspicious query detected from {Ip}: {Query}", context.Connection.RemoteIpAddress, query);
                    return;
                }

                // check small bodies (only if small)
                if (context.Request.ContentLength.HasValue && context.Request.ContentLength.Value < 1024 * 64) // 64KB
                {
                    context.Request.EnableBuffering();
                    using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
                    var body = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;
                    if (!string.IsNullOrWhiteSpace(body) && IsSuspicious(body))
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsync("Bad request");
                        _logger.LogWarning("Suspicious payload detected from {Ip}: {BodySnippet}", context.Connection.RemoteIpAddress, Truncate(body, 200));
                        return;
                    }
                }
            }

            // 8) Token blacklist check (if token present)
            if (_options.EnableTokenBlacklist && _tokenBlacklist != null && context.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var token = authHeader.ToString().Split(' ').LastOrDefault();
                if (!string.IsNullOrEmpty(token))
                {
                    if (await _tokenBlacklist.IsBlacklistedAsync(token))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Token revoked");
                        _logger.LogWarning("Revoked token used from {Ip}", context.Connection.RemoteIpAddress);
                        return;
                    }
                }
            }

            // all checks passed -> continue pipeline
            await _next(context);
        }

        private void AddSecurityHeaders(HttpContext context)
        {
            // HSTS - for 1 year by default
            context.Response.Headers["Strict-Transport-Security"] = $"max-age={(int)_options.HstsMaxAge.TotalSeconds}; includeSubDomains; preload";

            // Content Security Policy
            context.Response.Headers["Content-Security-Policy"] = _options.ContentSecurityPolicy;

            // X-Content-Type-Options
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";

            // X-Frame-Options
            context.Response.Headers["X-Frame-Options"] = "DENY";

            // Referrer-Policy
            context.Response.Headers["Referrer-Policy"] = "no-referrer";

            // Permissions-Policy (was Feature-Policy)
            context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";
        }

        private bool IsRateLimitExceeded(string ip)
        {

            var now = DateTime.UtcNow.Ticks;
            var q = _ipRequests.GetOrAdd(ip, _ => new ConcurrentQueue<long>());

            // push
            q.Enqueue(now);

            // pop older than window
            var windowTicks = _options.RateLimitWindow.Ticks;
            while (q.TryPeek(out var oldest) && (now - oldest) > windowTicks)
            {
                q.TryDequeue(out _);
            }

            return q.Count > _options.RequestsPerWindow;
        }

        private bool IsSuspicious(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;
            foreach (var r in _suspiciousRegexes)
            {
                if (r.IsMatch(input)) return true;
            }
            return false;
        }

        private static string Truncate(string s, int len)
            => s.Length <= len ? s : s.Substring(0, len) + "...";
    }

    // Extension method to register middleware
    public static class SecurityMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiSecurity(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SecurityMiddleware>();
        }
    }
}
