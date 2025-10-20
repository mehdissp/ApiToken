using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class Todo
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        //public int ProjectId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int StatusId { get; set; }
        public TodoPriority Priority { get; set; } = TodoPriority.Medium;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }

        public User User { get; set; } = null!;
       // public Project? Project { get; set; }
        public ICollection<TodoTag> TodoTags { get; set; } = new List<TodoTag>();
        public ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
    }
    //public enum TodoStatus
    //{
    //    Pending = 0,
    //    Completed = 1
    //}

    public enum TodoPriority
    {
        Low = 1,
        Medium = 2,
        High = 3
    }
}
