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
        public string? Color { get; set; }
        public string? DescriptionRows { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public Guid  UserId { get; set; }
        public bool IsDeleted { get; set; } = false;

        public ICollection<TodoTag> TodoTags { get; set; } = new List<TodoTag>();
        public ICollection<TagProject> TagProjects { get; set; } = new List<TagProject>();

        public Tag(string name,string? descriptionRows, string color,Guid userId)
        {
            Name = name;
            DescriptionRows = descriptionRows;
            Color = color;
            UserId = userId;
            CreatedAt = DateTime.Now;
        }

        public void UpdateTag(string name, string? descriptionRows, string color)
        {
            Name = name;
            DescriptionRows = descriptionRows;
            Color = color;
            UpdateAt = DateTime.Now;
        }
    }
}
