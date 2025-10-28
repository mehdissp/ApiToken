using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.DTOs
{
    public record RegisterDto(string Username, string Email, string Password);
    public record RegisterNewUserDto(string Username, string Email, string Password,bool IsActive,string MobileNumber,string fullname,string RoleId);
    public record UpdateNewUserDto(string UserId,string RoleId,string Username, string Email, string Password, bool IsActive, string MobileNumber, string fullname);
    public record GetNewUserDto(Guid Id,string Username, string Email, bool IsActive, string MobileNumber
        ,DateTime createdAt,string fullname,string roleName ,string roleId);
    public record LoginDto(string Username, string Password);
    public record RefreshDto(string Username, string RefreshToken);
}
