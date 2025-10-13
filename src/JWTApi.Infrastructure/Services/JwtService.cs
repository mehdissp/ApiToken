using JWTApi.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Services
{
    public class JwtService
    {
        private readonly IConfiguration _config;
        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        //public string GenerateToken(User user)
        //{
        //    var jwt = _config.GetSection("Jwt");
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    var claims = new[]
        //    {
        //        new Claim(JwtRegisteredClaimNames.Sub, user.Username),
        //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //             new Claim("id", user.Id.ToString()),
        //          new Claim(ClaimTypes.Name, user.Username)
        //    };

        //    var token = new JwtSecurityToken(
        //        issuer: jwt["Issuer"],
        //        audience: jwt["Audience"],
        //        claims: claims,
        //        expires: DateTime.UtcNow.AddSeconds(30),
        //        signingCredentials: creds
                
        //    );

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}
        public (string Token, DateTime ExpiresAt) GenerateToken(User user, List<string> roles)
{
    var jwt = _config.GetSection("Jwt");
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim("id", user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username)
    };
            //// اضافه کردن نقش‌ها و دسترسی‌ها
            //foreach (var role in user.UserRoles.Select(ur => ur.Role))
            //{
            //    claims.Add(new Claim(ClaimTypes.Role, role.Name));

            //    foreach (var perm in role.RolePermissions.Select(rp => rp.Permission))
            //    {
            //        claims.Add(new Claim("permission", perm.Code));
            //    }
            //}
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // زمان انقضا
            var expires = DateTime.UtcNow.AddMinutes(30);

    var token = new JwtSecurityToken(
        issuer: jwt["Issuer"],
        audience: jwt["Audience"],
        claims: claims,
        expires: expires,
        signingCredentials: creds
    );

    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

    // برگرداندن هم توکن و هم زمان انقضا
    return (tokenString, expires);
}

    }
}
