using JWTApi.Domain.Dtos.TodoStatus;
using JWTApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Interfaces
{
    public interface ITodoStatus
    {
        Task<List<TodoStatusDtos>> GetTodoStatus(int projectId, string roleId, CancellationToken cancellationToken);
        Task InsertTodoStatus(int projectId, string name, string color, CancellationToken cancellationToken);
        Task UpdateTodoStatus(int id, string name, string color, int? orderNum, CancellationToken cancellationToken);
        Task DeleteTodoStatus(int id, CancellationToken cancellationToken);

        Task<List<Tag>> GetTags(CancellationToken cancellationToken);
    }
}
