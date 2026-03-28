using JWTApi.Domain.Entities;

public class TodoWithTagsView
{
    public int Id { get; set; }
    public string UserNameCreator { get; set; }
    public string UserNameTodo { get; set; }

    public int? ProjectId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int StatusId { get; set; }
    public TodoPriority Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string UserIdTodo { get; set; }
    public int? CountComment { get; set; }
    public int? IsOverdute { get; set; }
    public bool DeleteButton { get; set; }
    public bool EditButton { get; set; }
    public string? Avatar { get; set; }
    public string? StatusName { get; set; }
    public string? StatusColor { get; set; }


    public List<TagDto> Tags { get; set; } = new List<TagDto>();
}

public class TagDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
}