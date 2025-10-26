using JWTApi.Domain.Dtos.Menu;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces.Menus;
using JWTApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Repositories.Menus
{
    public class MenuRepository : IMenuRepository
    {

        private readonly AppDbContext _context;
        public MenuRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;


        }

        public async Task<List<Menu>> GetMenuTreeAsync()
        {
            // خواندن مستقیم همه منوها از دیتابیس
            var allMenus = await _context.Menus
                .Include(m => m.Children)
                .ToListAsync();

            return BuildTree(allMenus);
        }

        private List<Menu> BuildTree(List<Menu> allMenus)
        {
            var rootMenus = allMenus.Where(x => x.ParentId == null).ToList();

            foreach (var rootMenu in rootMenus)
            {
                AddChildren(rootMenu, allMenus);
            }

            return rootMenus;
        }

        private void AddChildren(Menu parent, List<Menu> allMenus)
        {
            var children = allMenus.Where(x => x.ParentId == parent.Id).ToList();

            foreach (var child in children)
            {
                AddChildren(child, allMenus);
                parent.Children.Add(child);
            }
        }
    }
}
