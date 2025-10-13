using JWTApi.Domain.Interfaces;
using JWTApi.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JWTApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using JWTApi.Domain.Dtos;

namespace JWTApi.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
            => await _context.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
        public async Task<List<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken)
    =>  await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role) // برای دسترسی به نام نقش
            .Select(ur => ur.Role.Name)
            .ToListAsync();
        public async Task<User?> GetByUserIdAsync(string userId, CancellationToken cancellationToken)
       => await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId, cancellationToken);

        public async Task AddAsync(User user, CancellationToken cancellationToken)
        {
          await  _context.Users.AddAsync(user,cancellationToken );
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<List<MenuPermissionDto>> GetUserMenuPermissionsAsync(string userId, CancellationToken cancellationToken)
        {
            // نقش‌های کاربر
            var userRoleIds = await _context.UserRoles
                .Where(ur => ur.UserId.ToString() == userId)
                .Select(ur => ur.RoleId)
                .ToListAsync(cancellationToken);

            // گرفتن منوها و دسترسی‌ها
            var menuPermissions = await _context.Menus
                .Select(menu => new MenuPermissionDto
                {
                    MenuId = menu.Id,
                    MenuName = menu.Name,
                    Url = menu.Url,
                    Permissions = _context.RoleMenus
    .Where(rmp => userRoleIds.Contains(rmp.RoleId) && rmp.MenuId == menu.Id)
    .Select(rmp => rmp.Permission.Name)
    .Distinct()
    .ToList()

                })
                .ToListAsync(cancellationToken);

            return menuPermissions;
        }
        public async Task AddLoginAttemptAsync(LoginAttempt loginAttempt, CancellationToken cancellationToken)
        {
            await _context.LoginAttempts.AddAsync(loginAttempt, cancellationToken);
            await _context.SaveChangesAsync();
        }
        public async Task CheckAndLockIp(string? ip)
        {
            if (string.IsNullOrEmpty(ip))
                return;

            var window = DateTime.Now.AddMinutes(-5); 
            var failCount = await _context.LoginAttempts
                .CountAsync(x => x.IPAddress == ip && !x.Success && x.AttemptTime > window);

            if (failCount >= 5)
            {
            
                var alreadyLocked = await _context.IpLocks.AnyAsync(x => x.IPAddress == ip && x.LockEnd > DateTime.Now);
                if (!alreadyLocked)
                {
                    var lockEntry = new IpLock
                    {
                        IPAddress = ip,
                        LockEndAt = DateTime.Now,
                        LockEnd = DateTime.Now.AddMinutes(5), 
                        Reason = $"Too many failed login attempts ({failCount})"
                    };
                    _context.IpLocks.Add(lockEntry);
                    await _context.SaveChangesAsync();
                }
            }
        }


    }
}
