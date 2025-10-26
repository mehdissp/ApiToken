using JWTApi.Domain.Dtos.Menu;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces.Menus;
using JWTApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JWTApi.Infrastructure.Repositories.Menus
{
    public class MenuRepository : IMenuRepository
    {
        private readonly AppDbContext _context;

        public MenuRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task<List<MenuItem>> GetMenuTreeAsync()
        {
            // خواندن همه منوها بدون Include (برای جلوگیری از circular reference)
            var allMenus = await _context.Menus
                .AsNoTracking() // برای عملکرد بهتر

                .ToListAsync();

            return BuildTree(allMenus);
        }

        private List<MenuItem> BuildTree(List<Menu> allMenus)
        {
            // تبدیل به MenuItem و ساخت درخت
            var menuItems = allMenus.Select(m => new MenuItem
            {
                Id = m.Id,
                Name = m.Name,
                ParentId = m.ParentId,
                Url=m.Url,
                Children = new List<MenuItem>()
            }).ToList();

            var lookup = menuItems.ToDictionary(m => m.Id);
            var rootMenus = new List<MenuItem>();

            foreach (var menuItem in menuItems)
            {
                if (menuItem.ParentId.HasValue && lookup.ContainsKey(menuItem.ParentId.Value))
                {
                    lookup[menuItem.ParentId.Value].Children.Add(menuItem);
                }
                else
                {
                    rootMenus.Add(menuItem);
                }
            }

            return rootMenus;
        }
    }
}