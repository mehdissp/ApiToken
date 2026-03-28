using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace JWTApi.Api.ViewModels.TodoStatus
{
    public class TodoStatusViewModel
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Name { get; set; }
        [JsonProperty("color")]
        public string Color { get; set; }
        public int? OrderNum { get; set; }
    }
}
