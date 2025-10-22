using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces;
using JWTApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        //public async Task<List<TodoWithTagsView>> GetTodosWithTags(string userId,CancellationToken cancellationToken)
        //{
        //    var result = await _context.Todos
        //        .Include(t => t.TodoTags)
        //            .ThenInclude(tt => tt.Tag).Where(s=>s.UserId.ToString()== userId || s.UserTodo.ToString() == userId)
        //        .Select(async t => new TodoWithTagsView
        //        {
        //            Id = t.Id,
        //            UserNameCreator = _context.Users.Where(s=>s.UserId==t.UserId).Select(s=>s.FullName).FirstOrDefault(),
        //            UserNameTodo = _context.Users.Where(s => s.UserId == t.UserTodo).Select(s => s.FullName).FirstOrDefault(),
        //            Title = t.Title,
        //            Description = t.Description,
        //            StatusId = t.StatusId,
        //            Priority = (TodoPriority)t.Priority,
        //            CreatedAt = t.CreatedAt,
        //            DueDate = t.DueDate,
        //            CompletedAt = t.CompletedAt,
        //            Tags = t.TodoTags.Select(tt => new TagDto
        //            {
        //                Id = tt.Tag.Id,
        //                Name = tt.Tag.Name
        //            }).ToList()
        //        })
        //        .ToListAsync(cancellationToken);

        //    return result;
        //}
        //public async Task<List<TodoWithTagsView>> GetTodosWithTags(string userId, CancellationToken cancellationToken)
        //{
        //    var result = await _context.Todos
        //        .Include(t => t.TodoTags)
        //            .ThenInclude(tt => tt.Tag)
        //        .Where(t => t.UserId.ToString() == userId || t.UserTodo.ToString() == userId)
        //        .Select(t => new TodoWithTagsView
        //        {
        //            Id = t.Id,
        //            UserNameCreator = _context.Users
        //                .Where(u => u.UserId == t.UserId)
        //                .Select(u => u.FullName)
        //                .FirstOrDefault(),
        //            UserNameTodo = _context.Users
        //                .Where(u => u.UserId == t.UserTodo)
        //                .Select(u => u.FullName)
        //                .FirstOrDefault(),
        //            Title = t.Title,
        //            Description = t.Description,
        //            StatusId = t.StatusId,
        //            Priority = (TodoPriority)t.Priority,
        //            CreatedAt = t.CreatedAt,
        //            DueDate = t.DueDate,
        //            CompletedAt = t.CompletedAt,
        //            Tags = t.TodoTags.Select(tt => new TagDto
        //            {
        //                Id = tt.Tag.Id,
        //                Name = tt.Tag.Name
        //            }).ToList()
        //        })
        //        .ToListAsync(cancellationToken);

        //    return result;
        //}
        //public async InsertTodoTags(t)
        public async Task<List<TodoWithTagsView>> GetTodosWithTags(string userId, CancellationToken cancellationToken)
        {
            // ابتدا بررسی کنیم چه داده‌هایی در دیتابیس وجود دارد
            var todos = await _context.Todos
                .Where(t => t.UserId.ToString() == userId || t.UserTodo.ToString() == userId)
                .ToListAsync(cancellationToken);

            // لاگ بگیریم تا مقادیر را ببینیم
            foreach (var todo in todos)
            {
                Console.WriteLine($"TodoId: {todo.Id}, UserId: {todo.UserId}, UserTodo: {todo.UserTodo}");
            }
      
            // حالا کوئری اصلی
            var result = await _context.Todos
                .Include(t => t.TodoTags)
                    .ThenInclude(tt => tt.Tag)
                .Where(t => t.UserId.ToString() == userId || t.UserTodo.ToString() == userId)
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
                    Tags = t.TodoTags.Select(tt => new TagDto
                    {
                        Id = tt.Tag.Id,
                        Name = tt.Tag.Name
                    }).ToList()
                })
                .ToListAsync(cancellationToken);

            return result;
        }
    }
}
