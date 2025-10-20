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
    public class TodoStatusRepository : ITodoStatus
    {
        private readonly AppDbContext _context;
        public TodoStatusRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        public async Task DeleteTodoStatus(int id, CancellationToken cancellationToken)
        {
            TodoStatus todoStatus = await GetAsync(id, cancellationToken);
            todoStatus.IsDeleted = true;

        }

        public async Task<List<TodoStatus>> GetTodoStatus(int projectId, CancellationToken cancellationToken)
        {
            var t= await _context.TodoStatuses.Where(s => s.ProjectId == projectId && s.IsDeleted==false).OrderBy(s=>s.OrderNum).ToListAsync(cancellationToken);
            return t;
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
    }
}
