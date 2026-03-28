using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.DTOs.Tags
{
    //public class TagItemDtos
    // {
    //     public int? Id { get; set; }
    //     public string Name { get; set; }
    //     public string Desc { get; set; }
    //     [JsonProperty("color")]
    //     public string Color { get; set; }

    // }
    public class TagItemDtos
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Desc { get; set; }

        [JsonProperty("color")]
        public string? Color { get; set; } // nullable
    }
}
