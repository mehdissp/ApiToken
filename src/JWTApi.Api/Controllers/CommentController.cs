using JWTApi.Api.Response;
using JWTApi.Application.Services;
using JWTApi.Application.Services.Comment;
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
            var result = await _commentService.GetCommentDtoAsync(todoId, cancellationToken);

            return ResponseApi.Ok(result).ToHttpResponse();

        }


    }
}
