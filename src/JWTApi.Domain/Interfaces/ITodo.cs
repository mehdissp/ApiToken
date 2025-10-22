using JWTApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Interfaces
{
   public interface ITodo
    {
        Task<Todo> InsertTodo(Todo todo, CancellationToken cancellationToken);
        Task InsertTodoTags(List<TodoTag> todo, CancellationToken cancellationToken);
        Task<List<TodoWithTagsView>> GetTodosWithTags(string userId, CancellationToken cancellationToken);
    }
}
