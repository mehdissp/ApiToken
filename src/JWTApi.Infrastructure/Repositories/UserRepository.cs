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
using JWTApi.Infrastructure.Exceptions;

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

        public async Task AddUserWithAnotherUsers(User user,string userId,CancellationToken cancellationToken)
        {
            var currentProjects = await _context.Users.CountAsync(p => p.UserId.ToString() == userId, cancellationToken);
            // ۲. مجموع پروژه‌های مجاز از پکیج‌ها
            var totalFromPackages = await _context.UserPackages
                .Where(up => up.UserId.ToString() == userId)
                .Include(up => up.Package)
                .SumAsync(up => (int?)up.Package.MaxUsers) ?? 0;

            // ۳. مجموع پروژه‌های خرید اضافه
            var totalExtra = await _context.ExtraProjects
                .Where(ep => ep.UserId.ToString() == userId)
                .SumAsync(ep => (int?)ep.CountUsers) ?? 0;
            var totalAllowed = totalFromPackages + totalExtra;

            // ۴. بررسی محدودیت
            if (currentProjects >= totalAllowed)
            {
                throw new RestBasedException("شما به حداکثر تعداد کاربر مجاز خود رسیده‌اید.", 402);
            }
            user.UserId = Guid.Parse(userId);
            await _context.AddAsync(user, cancellationToken);
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
    //                Permissions = _context.RoleMenus
    //.Where(rmp => userRoleIds.Contains(rmp.RoleId) && rmp.MenuId == menu.Id )
    //.Select(rmp => rmp.Permission.Name)
    //.Distinct()
    //.ToList()

                })
                .ToListAsync(cancellationToken);

            return menuPermissions;
        }


        public async Task<List<MenuUi>> GetUserMenuPermissionsForUiAsync(string userId, CancellationToken cancellationToken)
        {
            // نقش‌های کاربر
            var userRoleIds = await _context.UserRoles
                .Where(ur => ur.UserId.ToString() == userId)
                .Select(ur => ur.RoleId)
                .ToListAsync(cancellationToken);

            // گرفتن منوها و دسترسی‌ها
            var menuPermissions = await _context.Menus.Where(s=>s.IsMenu==true && s.ParentId ==null)
                .Select(menu => new MenuUi
                {
                    Id = menu.Id,
                    Path = menu.Path,
                    Label = menu.Label,
                    Icon = menu.Icon,

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



        //public async Task<List<User>> GetUsersAsync(string userId, int PageNumber, int PageSize, CancellationToken cancellationToken)
        //{
        //    return await _context.Users.Where(s => s.UserId.ToString() == userId).ToListAsync(cancellationToken);
        //}
        public async Task<PagedResult<User>> GetUsersAsync(string userId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var query = _context.Users.Include(S=>S.UserPackages).ThenInclude(s=>s.Package).Where(s => s.UserId.ToString() == userId);

            var totalCount = await query.CountAsync(cancellationToken);

            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);



            var maxUsers = await _context.Users
    .Where(u => u.Id.ToString() ==userId)
    .SelectMany(u => u.UserPackages)
    .Select(up => up.Package.MaxUsers)
    .FirstOrDefaultAsync(cancellationToken);


            return new PagedResult<User>
            {
                Items = users,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Max= maxUsers
            };
        }


        public async Task<bool> checkUserNameDublicated(string userName,CancellationToken cancellationToken)
        {
            return await _context.Users.AnyAsync(s => s.Username == userName);
        }
        public async Task<bool> checkMobileDublicated(string mobileNumber, CancellationToken cancellationToken)
        {
            return await _context.Users.AnyAsync(s => s.MobileNumber == mobileNumber);
        }
        public async Task<bool> checkUserNameDublicatedUpdate(string userName,string userId, CancellationToken cancellationToken)
        {
            return await _context.Users.AnyAsync(s => s.Username == userName && s.Id.ToString()!=userId);
        }
        public async Task<bool> checkMobileDublicatedUpdate(string mobileNumber, string userId, CancellationToken cancellationToken)
        {
            return await _context.Users.AnyAsync(s => s.MobileNumber == mobileNumber && s.Id.ToString() != userId);
        }

        public async Task<List<Role>> GetRoleCombo(CancellationToken cancellationToken)
        {
            return await _context.Roles.Where(s => s.IsSeen == true).ToListAsync(cancellationToken);
        }
    }
}
