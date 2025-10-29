using JWTApi.Application.DTOs;
using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.ProjectUsers;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IUnitOfWork _unit;
        private readonly IPasswordHasher<User> _hasher;
        public UserService(IUserRepository userRepository, IPasswordHasher<User> hasher, IUnitOfWork unit)
        {
            _userRepo = userRepository;
            _hasher = hasher;
            _unit = unit;
        }
        public async Task<(bool Success, string Message)> RegisterAsync(RegisterNewUserDto dto, string userId, CancellationToken cancellationToken)
        {
            if (await _userRepo.GetByUsernameAsync(dto.Username, cancellationToken) != null)
                return (false, "User already exists");
            if (await _userRepo.checkMobileDublicated(dto.MobileNumber, cancellationToken) ==true)
                return (false, "MobileNumber already exists");

            var user = new User(dto.Username, dto.Email, dto.IsActive, dto.MobileNumber,dto.fullname);
            user.SetPassword(_hasher.HashPassword(user, dto.Password));
            await _userRepo.AddUserWithAnotherUsers(user, userId,dto.RoleId, cancellationToken);
            //await _unit.SaveChanges(cancellationToken);
            return (true, "User created successfully");
        }

        public async Task<(bool Success, string Message)> UpdateUserAsync(UpdateNewUserDto dto, string userId, CancellationToken cancellationToken)
        {
            if (await _userRepo.checkUserNameDublicatedUpdate(dto.Username,dto.UserId, cancellationToken) != null)
                return (false, "User already exists");
            if (await _userRepo.checkMobileDublicatedUpdate(dto.MobileNumber,dto.UserId, cancellationToken) == true)
                return (false, "MobileNumber already exists");
            var user = await _userRepo.GetByUserIdAsync(dto.UserId,cancellationToken);
            if (dto.IsChangePassword==true)
            {
                user.SetPassword(_hasher.HashPassword(user, dto.Password));
            }
            user =new User(dto.Username,dto.Email,dto.IsActive,dto.MobileNumber,dto.fullname)
    ;
            
            await _unit.SaveChanges(cancellationToken);
            return (true, "User created successfully");
        }

        public async Task<PagedResult<GetNewUserDto>> GetNewUser(string userId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var result = await _userRepo.GetUsersAsync(userId, pageNumber, pageSize, cancellationToken);

            var users = result.Items.Select(user =>
            {
                var userRole = user.UserRoles.FirstOrDefault();
                return new GetNewUserDto(
                    Id: user.Id,
                    Username: user.Username,
                    Email: user.Email,
                    IsActive: user.IsActive,
                    MobileNumber: user.MobileNumber,
                    createdAt: user.CreatedAt,
                    fullname: user.FullName,
                    roleId: userRole?.Role?.Id.ToString() ?? string.Empty,
                    roleName: userRole?.Role?.Name ?? string.Empty
                );
            }).ToList();

            return new PagedResult<GetNewUserDto>
            {
                Items = users,
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                Max = result.Max
            };
        }

        public async Task<List<Role>> GetRole(CancellationToken cancellationToken)
        {
            return await _userRepo.GetRoleCombo(cancellationToken);
        }

        public async Task<PagedResult<ProjectUserDtos>> GetProjectUserDtosAsync(string userId,int projectId,int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            return await _userRepo.GetProjectUserDtos(userId, projectId, pageNumber, pageSize, cancellationToken);
        }

    }
}
