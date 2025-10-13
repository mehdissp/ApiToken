using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class TodoTag
    {
        public int TodoId { get; set; }
        public int TagId { get; set; }

        public Todo Todo { get; set; } = null!;
        public Tag Tag { get; set; } = null!;
    }
}
