using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
   public class TagProject
    {
        public int ProjectId { get; set; }
        public int TagId { get; set; }

        [JsonIgnore]
        public Project Project { get; set; } = null!;
        [JsonIgnore]
        public Tag Tag { get; set; } = null!;
    }
}
