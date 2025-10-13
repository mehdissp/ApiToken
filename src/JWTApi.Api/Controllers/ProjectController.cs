using JWTApi.Api.Response;
using JWTApi.Application.DTOs;
using JWTApi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectController : ControllerBase
    {
        private readonly ProjectService _projectService;
        public ProjectController(ProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto, CancellationToken cancellationToken)
        {
            await _projectService.RegisterAsync(dto, cancellationToken);
           
              ? ResponseApi.Ok(message).ToHttpResponse()
              : Unauthorized();
        }


    }
}
