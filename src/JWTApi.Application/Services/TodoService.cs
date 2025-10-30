using JWTApi.Application.DTOs.Todo;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JWTApi.Application.Services
{
   public class TodoService
    {
        private ITodo _todo;
        private IUnitOfWork _unitOfWork;
        public TodoService(ITodo todo, IUnitOfWork unitOfWork)
        {
            _todo = todo;
            _unitOfWork = unitOfWork;
        }

        public async Task InsertTodo(TodoDtos todoDtos ,string userId,CancellationToken cancellationToken)
        {
            DateTime dateTime = new DateTime();
            if (todoDtos.DueDate is not null)
            {
                dateTime = ShamsiToMiladiConverter.ConvertShamsiToMiladi(todoDtos.DueDate);
            }
            if (todoDtos.UserId is null)
            {
                todoDtos.UserId = userId;
            }
            List<TodoTag> todoTag = new List<TodoTag>();
                var todo = new Todo(todoDtos.Title,todoDtos.Description,todoDtos.StatusId,userId,todoDtos.Priority, dateTime, todoDtos.UserId);
            var insertedTodo = await _todo.InsertTodo(todo, cancellationToken);
            await _unitOfWork.SaveChanges(cancellationToken);
            int todoId = insertedTodo.Id; // گرفتن Id
            if (todoDtos.todoTagsDtos != null && todoDtos.todoTagsDtos.Any())
            {
                var todoTags = todoDtos.todoTagsDtos.Select(tagDto => new TodoTag
                {
                    TodoId = todoId,
                    TagId = tagDto.Id // یا TagId اگر پراپرتی نامش متفاوت است
                }).ToList();

                await _todo.InsertTodoTags(todoTags, cancellationToken);
                await _unitOfWork.SaveChanges(cancellationToken);
            }



        }

        public async Task DeleteTodo(int id,string userId ,CancellationToken cancellationToken)
        {
            await _unitOfWork.CheckAccess(id, userId, cancellationToken);
            await _todo.DeleteAsync(id, userId, cancellationToken);
            await _unitOfWork.SaveChanges(cancellationToken);

        }


        public async Task<List<TodoWithTagsView>> TodoWithTagsViewsAsync(string userId,string roleId, CancellationToken cancellationToken)
        {
            return await _todo.GetTodosWithTags(userId, roleId, cancellationToken);
        }

        public async Task UpdateTodoWithStatusId(int id,string userId,int statusId,CancellationToken cancellation)
        {
            await _unitOfWork.CheckAccess(id, userId, cancellation);
            await _todo.UpdateTodoWithStatusId(id, statusId, cancellation);
            await _unitOfWork.SaveChanges(cancellation);

        }

        public async Task UpdateTodo(TodoEditDtos todoEdit,string userId, CancellationToken cancellation)
        {
            await _unitOfWork.CheckAccess(todoEdit.Id, userId, cancellation);
            DateTime dateTime = new DateTime();
            if (todoEdit.DueDate is not null)
            {
                dateTime = ShamsiToMiladiConverter.ConvertShamsiToMiladi(todoEdit.DueDate);
            }
            if (todoEdit.UserId is null)
            {
                todoEdit.UserId = userId;
            }
            await _todo.UpdateTodo(todoEdit.Id,todoEdit.Title,todoEdit.Description,todoEdit.StatusId,todoEdit.UserId
                ,todoEdit.Priority, dateTime, todoEdit.isArchive, userId, cancellation);
            var todoTags = todoEdit.todoTagsDtos.Select(tagDto => new TodoTag
            {
                TodoId = todoEdit.Id,
                TagId = tagDto.Id // یا TagId اگر پراپرتی نامش متفاوت است
            }).ToList();
            await _todo.UpdateTodoTags(todoTags, cancellation);
            await _unitOfWork.SaveChanges(cancellation);

        }
    }
}
