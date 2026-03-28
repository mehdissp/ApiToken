using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.TodoStatus
{
  public  class TodoStatusDtos
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public int ProjectId { get; set; }
        public int? OrderNum { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool ViewTodoStatus { get; set; }
        public bool DeleteTodoStatus { get; set; }
        public bool EditTodoStatus { get; set; }
        public bool InsertTodoStatus { get; set; }
    }
}
