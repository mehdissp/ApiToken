using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.DTOs.Todo
{
   public class TodoEditDtos
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int StatusId { get; set; }
        public int Priority { get; set; }
        public string? DueDate { get; set; }
        public string? UserId { get; set; }
        public  bool isArchive { get; set; }
        public List<TodoTagsDtos> todoTagsDtos { get; set; }
    }

}
