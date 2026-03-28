using JWTApi.Api.Response;
using JWTApi.Api.ViewModels;
using JWTApi.Api.ViewModels.Project;
using JWTApi.Api.ViewModels.Todo;
using JWTApi.Api.ViewModels.TodoStatus;
using JWTApi.Application.DTOs.Todo;
using JWTApi.Application.Services;
using JWTApi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TodoController : ControllerBase
    {
        private readonly TodoService _todoService;
        public TodoController(TodoService todoService)
        {
            _todoService = todoService;
        }

        [HttpPost("InsertTodo")]
        public async Task<IActionResult> InsertTodo([FromBody] TodoDtos todo, CancellationToken cancellationToken)
        {

            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _todoService.InsertTodo(todo,userId, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }


        [HttpPost("UpdateTodo")]
        public async Task<IActionResult> UpdateTodo([FromBody] TodoEditDtos todoEdit, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _todoService.UpdateTodo(todoEdit, userId, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }


        [HttpPost("DeleteTodo")]
        public async Task<IActionResult> DeleteTodo([FromBody] TodoDeleteViewModel todoDeleteViewModel, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _todoService.DeleteTodo(todoDeleteViewModel.Id, userId, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }


        [HttpGet("GetTodoWithTagsViewsAsync")]
        public async Task<IActionResult> GetTodoWithTagsViewsAsync( CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var roleId = User.Claims.FirstOrDefault(c => c.Type == "roleId")?.Value;
            var result = await _todoService.TodoWithTagsViewsAsync(userId, roleId, cancellationToken);

            return ResponseApi.Ok(result).ToHttpResponse();

        }

        [HttpPost("GetTodoWithTagsViewsAsyncArchive")]
        public async Task<IActionResult> GetTodoWithTagsViewsAsyncArchive([FromBody] PageSizeViewModel pageSize, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var roleId = User.Claims.FirstOrDefault(c => c.Type == "roleId")?.Value;
            var result = await _todoService.GetTodosWithTagsArchive((int)pageSize.Id,userId, roleId,pageSize.PageNumber,pageSize.PageSize, cancellationToken);

            return ResponseApi.Ok(result).ToHttpResponse();

        }


        [HttpPost("UpdateStatusTodo")]
        public async Task<IActionResult> UpdateStatusTodo([FromBody] TodoUpdateStatusViewModel todo, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _todoService.UpdateTodoWithStatusId(todo.Id, userId, todo.StatusId, cancellationToken);
            return ResponseApi.Ok().ToHttpResponse();
        }


    }
}
