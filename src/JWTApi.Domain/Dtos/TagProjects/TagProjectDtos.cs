using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.TagProjects
{
  public  class TagProjectDtos
    {
        public int Id { get; set; }
        public string TagName { get; set; }
        public string Color { get; set; }
        public bool IsCheck { get; set; }
    }
}
