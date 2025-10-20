using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Color { get; set; }
        public ICollection<TodoTag> TodoTags { get; set; } = new List<TodoTag>();
    }
}
