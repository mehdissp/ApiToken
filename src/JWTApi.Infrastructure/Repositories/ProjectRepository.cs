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
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _context;
        public ProjectRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(string name,string userId, CancellationToken cancellationToken)
        {
            var currentProjects = await _context.Projects.CountAsync(p => p.UserId.ToString() == userId, cancellationToken);

            // ۲. مجموع پروژه‌های مجاز از پکیج‌ها
            var totalFromPackages = await _context.UserPackages
                .Where(up => up.UserId.ToString() == userId)
                .Include(up => up.Package)
                .SumAsync(up => (int?)up.Package.MaxProjects) ?? 0;

            // ۳. مجموع پروژه‌های خرید اضافه
            var totalExtra = await _context.ExtraProjects
                .Where(ep => ep.UserId.ToString() == userId)
                .SumAsync(ep => (int?)ep.CountProject) ?? 0;
            var totalAllowed = totalFromPackages + totalExtra;
            // ۴. بررسی محدودیت
            if (currentProjects > totalAllowed)
            {
                throw new RestBasedException("شما به حداکثر تعداد پروژه مجاز خود رسیده‌اید.", 403);
            }

            Project project = new Project();
            project.Name = name;
            project.UserId = Guid.Parse(userId);
            await _context.AddAsync(project,cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(int projectId, CancellationToken cancellationToken)
        {
            // گرفتن پروژه به همراه بررسی وجود تو دوها

            var project = await _context.Projects
                .Include(p => p.Todos)
                .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);

            if (project == null)
                throw new RestBasedException(ApiErrorCodeMessage.Error_NotFound);

            if (project.Todos != null && project.Todos.Any())
                throw new RestBasedException(ApiErrorCodeMessage.Error_Refrence);

            // حذف پروژه
            //_context.Projects.Remove(project);
            project.IsDeleted = true;

            await _context.SaveChangesAsync(cancellationToken);
        }


        public async Task<Project?> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken)
        {
            return await _context.Projects.FindAsync(projectId, cancellationToken);
        }

        public async Task<(List<ProjectWithPackageInfoDto> Items, int TotalCount, int TotalPages)> GetProjectsWithPackageInfo(
            string userId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var skip = (pageNumber - 1) * pageSize;

            var baseQuery = _context.Projects
                .Where(s => s.UserId.ToString() == userId && s.IsDeleted==false)
                .Include(s => s.User)
                    .ThenInclude(u => u.UserPackages)
                    .ThenInclude(up => up.Package);

            var totalCount = await baseQuery.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var query = baseQuery
                .OrderByDescending(s => s.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
                .Select(s => new
                {
                    Project = s,
                    MaxProjects = s.User.UserPackages.FirstOrDefault().Package.MaxProjects
                });

            var result = await query.ToListAsync(cancellationToken);
            

            var items = result.Select((item, index) => new ProjectWithPackageInfoDto
            {
                Id=item.Project.Id,
                Name = item.Project.Name,
                CreatedAt = item.Project.CreatedAt,
                MaxProjects = item.MaxProjects,
                RowNum = skip + index + 1
            }).ToList();

            return (items, totalCount, totalPages);
        }
    }
}
