using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
   public class TodoStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public int ProjectId { get; set; }
        public int? OrderNum { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Project? Project { get; set; }
    }
}
