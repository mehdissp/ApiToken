using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
   public class Comment
    {
        public int Id { get; set; }
        public int TodoId { get; set; }
        public string Message { get; set; }
        public Guid UserId { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public bool IsSeen { get; set; } = false;
        public DateTime? SeenAt { get; set; }
        public Todo? Todo { get; set; }

        private Comment() { }

        public Comment(int todoId,string message,string userId)
        {
            TodoId = todoId;
            Message = message;
            UserId = Guid.Parse(userId);
        }


    }
}
