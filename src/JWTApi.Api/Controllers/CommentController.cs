using JWTApi.Api.Response;
using JWTApi.Api.ViewModels.Comments;
using JWTApi.Application.DTOs.Todo;
using JWTApi.Application.Services;
using JWTApi.Application.Services.Comments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly CommentService  _commentService;
        public CommentController(CommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet("GetCommentDtoAsync")]
        public async Task<IActionResult> GetCommentDtoAsync([FromQuery] int todoId, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var result = await _commentService.GetCommentDtoAsync(todoId, userId, cancellationToken);

            return ResponseApi.Ok(result).ToHttpResponse();

        }

        [HttpPost("InsertComment")]
        public async Task<IActionResult> InsertComment([FromBody] CommentInsertViewModel commentInsertViewModel, CancellationToken cancellationToken)
        {

            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _commentService.InsertComment(commentInsertViewModel.Message, userId, commentInsertViewModel.TodoId, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }

        [HttpDelete("DeleteComment")]
        public async Task<IActionResult> DeleteComment([FromQuery] int id, CancellationToken cancellationToken)
        {

            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _commentService.DeleteComment(id,userId, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }

        [HttpDelete("UpdateSeenComment")]
        public async Task<IActionResult> UpdateSeenComment([FromQuery] int todoId, CancellationToken cancellationToken)
        {

            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _commentService.UpdateSeenFlag(todoId, userId, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }



    }
}
