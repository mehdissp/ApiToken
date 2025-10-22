using JWTApi.Api.Response;
using JWTApi.Api.ViewModels.TodoStatus;
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

        [HttpGet("GetTodoWithTagsViewsAsync")]
        public async Task<IActionResult> GetTodoWithTagsViewsAsync( CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var result = await _todoService.TodoWithTagsViewsAsync(userId, cancellationToken);

            return ResponseApi.Ok(result).ToHttpResponse();

        }

        
    }
}
