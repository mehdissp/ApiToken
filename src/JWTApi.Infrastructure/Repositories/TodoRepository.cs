using JWTApi.Domain.Dtos;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces;
using JWTApi.Infrastructure.Data;
using JWTApi.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Repositories
{
    public class TodoRepository : ITodo
    {
        private readonly AppDbContext _context;
        public TodoRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Todo> InsertTodo(Todo todo, CancellationToken cancellationToken)
        {
            await _context.Todos.AddAsync(todo, cancellationToken);
            return todo;
        }

        public async Task InsertTodoTags(List<TodoTag> todo, CancellationToken cancellationToken)
        {
            await _context.TodoTags.AddRangeAsync(todo, cancellationToken);
         
        }

     
        public async Task<List<TodoWithTagsView>> GetTodosWithTags(string userId, CancellationToken cancellationToken)
        {

            // حالا کوئری اصلی
            var result = await _context.Todos
                .Include(t => t.TodoTags)
                    .ThenInclude(tt => tt.Tag)
                .Where(t => (t.UserId.ToString() == userId || t.UserTodo.ToString() == userId) && t.IsDeleted==false && t.IsArchive==false)
                .Select(t => new TodoWithTagsView
                {
                    Id = t.Id,
                    UserNameCreator = _context.Users
                        .Where(u => u.Id.ToString() == t.UserId.ToString())
                        .Select(u => u.FullName)
                        .First() ?? "نامشخص",
                    UserNameTodo = t.UserTodo != null ?
                        _context.Users
                            .Where(u => u.Id.ToString() == t.UserTodo.ToString())
                            .Select(u => u.FullName)
                            .First() ?? "نامشخص"
                        : null,
                    Title = t.Title,
                    Description = t.Description,
                    StatusId = t.StatusId,
                    Priority = (TodoPriority)t.Priority,
                    CreatedAt = t.CreatedAt,
                    DueDate = t.DueDate,
                    CompletedAt = t.CompletedAt,
                    UserIdTodo=t.UserTodo.ToString(),
                    DeleteButton=t.UserId.ToString()== userId ?true :false,
                    EditButton = t.UserId.ToString() == userId ? true : false,
                    CountComment =_context.Comments.Where(s=>s.TodoId==t.Id && s.IsDeleted==false).Count(),

                    IsOverdute = t.DueDate == null ? 0 :
                        t.DueDate <= DateTime.Now.AddDays(1) ? 1 :
                        t.DueDate <= DateTime.Now.AddDays(20) ? 2 : 3,
                    Tags = t.TodoTags.Select(tt => new TagDto
                    {
                        Id = tt.Tag.Id,
                        Name = tt.Tag.Name,
                        Color=tt.Tag.Color
                    }).ToList()
                })
                .ToListAsync(cancellationToken);

            return result;
        }


        public async Task DeleteAsync(int id,string userId, CancellationToken cancellationToken)
        {
            // گرفتن پروژه به همراه بررسی وجود تو دوها

            var todo = await GetTodoAsync(id,cancellationToken);

            if (todo == null)
                throw new RestBasedException(ApiErrorCodeMessage.Error_NotFound);
            if (todo.UserId.ToString() !=userId)
            {
                throw new RestBasedException(ApiErrorCodeMessage.Error_Access);
            }
            todo.IsDeleted = true;
        }
        public async Task<Todo?> GetTodoAsync(int id,CancellationToken cancellation)
        {
            return  await _context.Todos
                .FirstOrDefaultAsync(p => p.Id == id, cancellation);
        }

        public async Task UpdateTodoWithStatusId(int id,int statusId,CancellationToken cancellationToken)
        {
            var todo = await GetTodoAsync(id, cancellationToken);
            todo.StatusId = statusId;
        }

        public async Task UpdateTodo(int id ,string title,string desc,int statusId,string userIdTodo,int priority
            ,DateTime dueDate,bool isArchive,string userId 
            , CancellationToken cancellationToken)
        {
            var todo = await GetTodoAsync(id, cancellationToken);
            todo.DueDate = dueDate;
            todo.StatusId = statusId;
            todo.UserTodo = Guid.Parse(userIdTodo);
            todo.Title = title;
            todo.Priority = (TodoPriority)priority;
            todo.Description = desc;
            if (todo.UserId.ToString() != userId)
            {
                throw new RestBasedException(ApiErrorCodeMessage.Error_Access);
            }
            todo.IsArchive = isArchive;
        }

        public async Task UpdateTodoTags(List<TodoTag> newTodoTags, CancellationToken cancellationToken)
        {
            if (!newTodoTags.Any()) return;

            var todoId = newTodoTags.First().TodoId;

            // گرفتن فقط تگ‌های مربوط به این TodoId
            var existingTodoTags = await _context.TodoTags
                .Where(t => t.TodoId == todoId)
                .ToListAsync(cancellationToken);

            // ایجاد HashSet از TagIdهای موجود و جدید
            var existingTagIds = new HashSet<int>(existingTodoTags.Select(t => t.TagId));
            var newTagIds = new HashSet<int>(newTodoTags.Select(t => t.TagId));

            // اضافه کردن تگ‌های جدید
            var tagsToAdd = newTodoTags
                .Where(t => !existingTagIds.Contains(t.TagId))
                .ToList();

            // حذف تگ‌های قدیمی
            var tagsToRemove = existingTodoTags
                .Where(t => !newTagIds.Contains(t.TagId))
                .ToList();

            if (tagsToAdd.Any())
            {
                await _context.TodoTags.AddRangeAsync(tagsToAdd, cancellationToken);
            }

            if (tagsToRemove.Any())
            {
                _context.TodoTags.RemoveRange(tagsToRemove);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

      

    }
}
