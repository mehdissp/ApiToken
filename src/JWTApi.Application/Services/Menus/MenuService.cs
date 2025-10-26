using JWTApi.Domain.Dtos.Menu;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.Services.Menus
{
   public class MenuService
    {
        private IMenuRepository _menuRepository;
        public MenuService(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public async Task<List<Menu>> MenuItemsAsync()
        {
            return await _menuRepository.GetMenuTreeAsync();
        }
    }
}
