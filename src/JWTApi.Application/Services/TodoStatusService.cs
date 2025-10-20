using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.Services
{
   public class TodoStatusService
    {
        private readonly ITodoStatus _todoStatus;
        private readonly IUnitOfWork _unit;
        public TodoStatusService(ITodoStatus todoStatus, IUnitOfWork unit   )
        {
            _todoStatus = todoStatus;
            _unit = unit;
        }
        public async Task<List<TodoStatus>> GetTodoStatusesAsync(int projectId,CancellationToken cancellationToken)
        {
            return await _todoStatus.GetTodoStatus(projectId, cancellationToken);
        }
        public async Task InsertTodoStatus(int projectId,string name,string color,CancellationToken cancellationToken)
        {
            await _todoStatus.InsertTodoStatus(projectId, name, color, cancellationToken);
            await _unit.SaveChanges(cancellationToken);
        }
        public async Task UpdateTodoStatus(int id, string name, string color,int? orderNum, CancellationToken cancellationToken)
        {
            await _todoStatus.UpdateTodoStatus(id, name, color, orderNum, cancellationToken);
            await _unit.SaveChanges(cancellationToken);
        }
        public async Task DeleteTodoStatus(int id, CancellationToken cancellationToken)
        {
            await _todoStatus.DeleteTodoStatus(id,  cancellationToken);
            await _unit.SaveChanges(cancellationToken);
        }
    }
}
