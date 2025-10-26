using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.Comment
{
   public class CommentDto
    {
        public int Id { get; set; }
        public int TodoId { get; set; }
        public string UserAuthor { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
        public bool ShowDeleted { get; set; }
        public DateTime? SeenAt { get; set; }

    }
}
