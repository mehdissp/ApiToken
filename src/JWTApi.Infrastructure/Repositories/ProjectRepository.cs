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
            Project project = new Project;
            project.Name = name;
            project.UserId = userId;
            await _context.AddAsync(project,cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var checkTodo = await _context.Todos.AnyAsync(s => s.ProjectId == id,cancellationToken);
            if (!checkTodo)
            {
                var project =await GetByProjectIdAsync(id,cancellationToken);
                _context.Remove(project);
            }
            else
            {
                throw new RestBasedException(ApiErrorCodeMessage.Error_Refrence);
            }
        }

        public async Task<Project?> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken)
        {
            return await _context.Projects.FindAsync(projectId, cancellationToken);
        }

       
    }
}
