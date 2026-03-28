using JWTApi.Api.Response;
using JWTApi.Api.ViewModels;
using JWTApi.Api.ViewModels.Todo;
using JWTApi.Api.ViewModels.TodoStatus;
using JWTApi.Application.DTOs.Tags;
using JWTApi.Application.DTOs.Todo;
using JWTApi.Application.Services;
using JWTApi.Application.Services.Tags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TagController : ControllerBase
    {
        private readonly TagService _tagService;
        public TagController(TagService tagService)
        {
            _tagService = tagService;
        }

        [HttpPost("InsertTag")]
        public async Task<IActionResult> InsertTag([FromBody] TagItemDtos tag, CancellationToken cancellationToken)
        {
            if (!tag.Color.StartsWith("#") && !string.IsNullOrEmpty(tag.Color))
            {
                tag.Color = "#" + tag.Color;
            }
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _tagService.InsertTag(tag, userId, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }

        [HttpPost("UpdateTag")]
        public async Task<IActionResult> UpdateTag([FromBody] TagItemDtos tag, CancellationToken cancellationToken)
        {
            if (!tag.Color.StartsWith("#") && !string.IsNullOrEmpty(tag.Color))
            {
                tag.Color = "#" + tag.Color;
            }
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _tagService.UpdateTags(tag, userId, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }

        [HttpPost("DeleteTag")]
        public async Task<IActionResult> DeleteTag([FromBody] TodoDeleteViewModel todoDeleteViewModel, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _tagService.DeleteTags(todoDeleteViewModel.Id, userId, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }

        [HttpPost("GetTags")]
        public async Task<IActionResult> GetTags([FromBody] PageSizeViewModel pageSize, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

            var result = await _tagService.GetTagsAsync(userId,pageSize.PageNumber,pageSize.PageSize, pageSize.KeyValue, cancellationToken);
            var response = new
            {
                Items = result.Tags,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPage,
            };
            return ResponseApi.Ok(response).ToHttpResponse();

        }

        [HttpPost("GetTagProject")]
        public async Task<IActionResult> GetTagProject([FromBody] PageSizeViewModel pageSize, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var result = await _tagService.GetTagProjectDtos(userId, (int)pageSize.Id, pageSize.PageNumber, pageSize.PageSize, cancellationToken);
            var response = new
            {
                Items = result.Items,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages,

            };
            return ResponseApi.Ok(response).ToHttpResponse();

        }


    }
}
