using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class Reminder
    {
        public int Id { get; set; }
        public int TodoId { get; set; }
        public DateTime ReminderTime { get; set; }
        public bool IsSent { get; set; } = false;

        public Todo Todo { get; set; } = null!;
    }
}
