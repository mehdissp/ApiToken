using JWTApi.Api.Response;
using JWTApi.Api.ViewModels;
using JWTApi.Application.Services;
using JWTApi.Application.Services.Menus;
using JWTApi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MenuController : ControllerBase
    {
        private MenuService _menu;
        public MenuController(MenuService menu)
        {
            _menu = menu;
        }

        [HttpGet("GetMenuItemsAsync")]
        public async Task<IActionResult> GetMenuItemsAsync( CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
                var result = await _menu.MenuItemsAsync();

                return ResponseApi.Ok(result).ToHttpResponse();
            }
            catch (Exception ex)
            {

                throw;
            }
  

        }
    }
}
