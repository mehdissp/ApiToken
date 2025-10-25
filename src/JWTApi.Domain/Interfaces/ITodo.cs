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
        Task DeleteAsync(int id, string userId, CancellationToken cancellationToken);
        Task<List<TodoWithTagsView>> GetTodosWithTags(string userId, CancellationToken cancellationToken);
        Task<Todo?> GetTodoAsync(int id, CancellationToken cancellation);
        Task UpdateTodoWithStatusId(int id, int statusId, CancellationToken cancellationToken);
        Task UpdateTodo(int id, string title, string desc, int statusId, string userIdTodo, int priority
            , DateTime dueDate,bool isArchive, string userId
            , CancellationToken cancellationToken);

        Task UpdateTodoTags(List<TodoTag> newTodoTags, CancellationToken cancellationToken);

    }
}
