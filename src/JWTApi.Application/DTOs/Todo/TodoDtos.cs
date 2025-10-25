using JWTApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.DTOs.Todo
{
  public  class TodoDtos
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int StatusId { get; set; }
        public int Priority { get; set; } 
        public string? DueDate { get; set; }
        public string? UserId { get; set; }
        public List<TodoTagsDtos> todoTagsDtos { get; set; }
    }

    public class TodoTagsDtos
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
