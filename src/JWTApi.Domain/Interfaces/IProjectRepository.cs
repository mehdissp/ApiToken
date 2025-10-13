using JWTApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Interfaces
{
    public interface IProjectRepository
    {
        Task AddAsync(Project project, CancellationToken cancellationToken);

        Task DeleteAsync(int id, CancellationToken cancellationToken);
        Task<Project?> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken);
    }
}
