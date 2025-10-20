using JWTApi.Api.Response;
using JWTApi.Api.ViewModels;
using JWTApi.Api.ViewModels.Project;
using JWTApi.Api.ViewModels.TodoStatus;
using JWTApi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace JWTApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class TodoStatusController : ControllerBase
    {
        private readonly TodoStatusService _todoStatus;
        public TodoStatusController(TodoStatusService todoStatusService)
        {
            _todoStatus = todoStatusService;
        }

        [HttpGet("GetTodoStatus")]
        public async Task<IActionResult> GetTodoStatus([FromQuery] int projectId, CancellationToken cancellationToken)
        {
            //var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var result = await _todoStatus.GetTodoStatusesAsync(projectId,cancellationToken);
       
            return ResponseApi.Ok(result).ToHttpResponse();

        }

        [HttpPost("InsertTodoStatus")]
        public async Task<IActionResult> InsertTodoStatus([FromBody] TodoStatusViewModel todoStatusViewModel, CancellationToken cancellationToken)
        {
            if (!todoStatusViewModel.Color.StartsWith("#") && !string.IsNullOrEmpty(todoStatusViewModel.Color))
            {
                todoStatusViewModel.Color = "#" + todoStatusViewModel.Color;
            }
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _todoStatus.InsertTodoStatus(todoStatusViewModel.ProjectId, todoStatusViewModel.Name
                , todoStatusViewModel.Color, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }

        [HttpPost("UpdateTodoStatus")]
        public async Task<IActionResult> UpdateTodoStatus([FromBody] TodoStatusViewModel todoStatusViewModel, CancellationToken cancellationToken)
        {
            if (!todoStatusViewModel.Color.StartsWith("#") && !string.IsNullOrEmpty(todoStatusViewModel.Color))
            {
                todoStatusViewModel.Color = "#" + todoStatusViewModel.Color;
            }
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _todoStatus.UpdateTodoStatus(todoStatusViewModel.Id, todoStatusViewModel.Name
                , todoStatusViewModel.Color,todoStatusViewModel.OrderNum, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }

        [HttpDelete("DeleteTodoStatus")]
        public async Task<IActionResult> DeleteTodoStatus([FromQuery] int id, CancellationToken cancellationToken)
        {
      
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _todoStatus.DeleteTodoStatus(id, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }


    }
}
