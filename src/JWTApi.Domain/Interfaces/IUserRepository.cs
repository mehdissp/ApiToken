using JWTApi.Domain.Dtos;
using JWTApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken);
        Task AddAsync(User user, CancellationToken cancellationToken);
        Task UpdateAsync(User user );
        Task<User?> GetByUserIdAsync(string userId, CancellationToken cancellationToken);
        Task<List<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken);
        Task<List<MenuPermissionDto>> GetUserMenuPermissionsAsync(string userId,CancellationToken cancellationToken);
         Task AddLoginAttemptAsync(LoginAttempt loginAttempt, CancellationToken cancellationToken);
        Task CheckAndLockIp(string? ip);
        Task<List<MenuUi>> GetUserMenuPermissionsForUiAsync(string userId, CancellationToken cancellationToken);
    }
}
