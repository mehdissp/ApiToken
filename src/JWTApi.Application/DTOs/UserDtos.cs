using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.DTOs
{
    public record RegisterDto(string Username, string Email, string Password);
    public record LoginDto(string Username, string Password);
    public record RefreshDto(string Username, string RefreshToken);
}
