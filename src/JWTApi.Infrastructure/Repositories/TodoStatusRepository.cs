using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.TodoStatus;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces;
using JWTApi.Infrastructure.Data;
using JWTApi.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Repositories
{
    public class TodoStatusRepository : ITodoStatus
    {
        private readonly AppDbContext _context;
        public TodoStatusRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        public async Task DeleteTodoStatus(int id, CancellationToken cancellationToken)
        {
            var check = await _context.Todos.Where(s => s.StatusId == id).CountAsync(cancellationToken);
            if (check >0)
            {
                throw new RestBasedException(ApiErrorCodeMessage.Error_Refrence);
            }
            TodoStatus todoStatus = await GetAsync(id, cancellationToken);
            todoStatus.IsDeleted = true;

        }

        public async Task<List<TodoStatusDtos>> GetTodoStatus(int projectId,string roleId, CancellationToken cancellationToken)
        {
            var checkDeleteTodoStatus = await HasMenuAccessAsync(roleId, "/api/TodoStatus/DeleteTodoStatus", cancellationToken);
            var checkEditTodoStatus = await HasMenuAccessAsync(roleId, "/api/TodoStatus/UpdateTodoStatus", cancellationToken);
            var checkInsertTodoStatus = await HasMenuAccessAsync(roleId, "/api/TodoStatus/InsertTodoStatus", cancellationToken);
            var checkViewTodoStatus = await HasMenuAccessAsync(roleId, "/api/TodoStatus/GetTodoStatus", cancellationToken);

            var t = await _context.TodoStatuses.Where(s => s.ProjectId == projectId && s.IsDeleted==false).OrderBy(s=>s.OrderNum)
                .Select(s=>new TodoStatusDtos
                {
                    Id=s.Id,
                    ProjectId=s.ProjectId,
                    Color=s.Color,
                    Name=s.Name,
                    CreatedAt=s.CreatedAt,
                    OrderNum=s.OrderNum,
                    ViewTodoStatus= checkViewTodoStatus,
                    DeleteTodoStatus= checkDeleteTodoStatus,
                    EditTodoStatus= checkEditTodoStatus,
                    InsertTodoStatus= checkInsertTodoStatus,
                    

                } )
                .ToListAsync(cancellationToken);
            return t;
        }
        private async Task<bool> HasMenuAccessAsync(string roleId, string menuUrl, CancellationToken cancellationToken)
        {
            var menu = await _context.Menus
                .Where(s => s.Url == menuUrl)
                .FirstOrDefaultAsync(cancellationToken);

            if (menu == null) return false;

            return await _context.RoleMenus
                .AnyAsync(s => s.RoleId.ToString() == roleId && s.MenuId == menu.Id, cancellationToken);
        }

        public async Task<TodoStatus> GetAsync(int id ,CancellationToken cancellationToken)
        {
            return await _context.TodoStatuses.FindAsync(id, cancellationToken);
        }

        public async Task InsertTodoStatus(int projectId, string name, string color, CancellationToken cancellationToken)
        {
            var maxOrderNum = await _context.TodoStatuses.Where(s=>s.ProjectId==projectId && s.IsDeleted==false).MaxAsync(s => (int?)s.OrderNum, cancellationToken);
            var num = (maxOrderNum ?? 0) + 1;


            TodoStatus todoStatus = new TodoStatus();
            todoStatus.ProjectId = projectId;
            todoStatus.Name = name;
            todoStatus.Color = color;
            todoStatus.OrderNum = num;
            await _context.TodoStatuses.AddAsync(todoStatus, cancellationToken);
        }

        public async Task UpdateTodoStatus(int id, string name, string color,int? orderNum, CancellationToken cancellationToken)
        {
            //TodoStatus todoStatus = await GetAsync(id, cancellationToken);
            //todoStatus.Color = color;
            //todoStatus.Name = name;
            //todoStatus.OrderNum = orderNum;

            TodoStatus todoStatus = await GetAsync(id, cancellationToken);
            int? oldOrderNum = todoStatus.OrderNum;

            // اگر مقدار جدید با مقدار قدیمی یکسان باشد، نیاز به تغییر نیست
            if (orderNum == oldOrderNum)
            {
                // فقط سایر فیلدها را آپدیت کن
                todoStatus.Color = color;
                todoStatus.Name = name;
         
                return;
            }

            if (orderNum < oldOrderNum)
            {
                // عدد جدید کوچکتر از عدد قدیمی است
                // تمام آیتم‌هایی که OrderNum بین عدد جدید و عدد قدیمی هستند (شامل عدد جدید) باید +1 شوند
                var itemsToUpdate = await _context.TodoStatuses
                    .Where(x => x.OrderNum >= orderNum && x.OrderNum < oldOrderNum && x.Id != id)
                    .ToListAsync(cancellationToken);

                foreach (var item in itemsToUpdate)
                {
                    item.OrderNum += 1;
                }
            }
            else // orderNum > oldOrderNum
            {
                // عدد جدید بزرگتر از عدد قدیمی است
                // تمام آیتم‌هایی که OrderNum بین عدد قدیمی و عدد جدید هستند (شامل عدد جدید) باید -1 شوند
                var itemsToUpdate = await _context.TodoStatuses
                    .Where(x => x.OrderNum > oldOrderNum && x.OrderNum <= orderNum && x.Id != id)
                    .ToListAsync(cancellationToken);

                foreach (var item in itemsToUpdate)
                {
                    item.OrderNum -= 1;
                }
            }

            // آپدیت آیتم جاری
            todoStatus.Color = color;
            todoStatus.Name = name;
            todoStatus.OrderNum = orderNum;

        }

        public async Task<List<Tag>> GetTags(CancellationToken cancellationToken)
        {
            return await _context.Tags.ToListAsync(cancellationToken);
        }
    }
}
