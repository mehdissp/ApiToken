using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;

        //public User User { get; set; } = null!;
        public ICollection<Todo> Todos { get; set; } = new List<Todo>();
        public ICollection<TodoStatus> TodoStatuses { get; set; } = new List<TodoStatus>();
        public ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();
    }
}
