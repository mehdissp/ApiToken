using JWTApi.Api.Response;
using JWTApi.Api.ViewModels;
using JWTApi.Api.ViewModels.Project;
using JWTApi.Application.DTOs;
using JWTApi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  //  [Authorize]
    public class ProjectController : ControllerBase
    {
        private readonly ProjectService _projectService;
        public ProjectController(ProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpPost("InsertProject")]
        public async Task<IActionResult> InsertProject([FromBody] ProjectAddViewModel projectAddViewModel, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _projectService.InsertProject(projectAddViewModel.Name, userId, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();
             
        }
        [HttpPost("DeleteProject")]
        public async Task<IActionResult> DeleteProject([FromBody] ProjectDeleteViewModel projectAddViewModel, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _projectService.DeleteProject(projectAddViewModel.Id, userId, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }


        [HttpPost("GetProject")]
        public async Task<IActionResult> GetProject([FromBody]PageSizeViewModel pageSize, CancellationToken cancellationToken)
        {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
                var result = await _projectService.GetProjectsAsync(userId, pageSize.PageNumber, pageSize.PageSize, cancellationToken);
                var response = new
                {
                    Items = result.Items,
                    TotalCount = result.TotalCount,
                    TotalPages = result.TotalPages
                };
                return ResponseApi.Ok(response).ToHttpResponse();
        
        }

    }
}
