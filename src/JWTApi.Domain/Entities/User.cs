using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Name { get; set; } = string.Empty;
        public string Username { get; private set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public Guid? UserId { get; set; }
        public bool IsActive { get; set; } = true;

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<Todo> Todos { get; set; } = new List<Todo>();
        public ICollection<Project> Projects { get; set; } = new List<Project>();
        public ICollection<ExtraProject> ExtraProjects { get; set; } = new List<ExtraProject>();
        public ICollection<UserPackage> UserPackages { get; set; } = new List<UserPackage>();

        private User() { }

        public User(string username, string email)
        {
            Id = Guid.NewGuid();
            Username = username;
            Email = email;
        }

        public User(string username, string email, bool isActive,string mobileNumber,string fullName)
        {
            Id = Guid.NewGuid();
            Username = username;
            Email = email;
            MobileNumber = mobileNumber;
            IsActive = isActive;
            FullName = fullName;

        }

        public void SetPassword(string hash)
        {
            PasswordHash = hash;
        }
    }
}
