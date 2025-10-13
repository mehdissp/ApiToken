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
            _context.Projects.Remove(project);

            await _context.SaveChangesAsync(cancellationToken);
        }


        public async Task<Project?> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken)
        {
            return await _context.Projects.FindAsync(projectId, cancellationToken);
        }

       
    }
}
