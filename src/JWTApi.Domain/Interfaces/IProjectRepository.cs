using JWTApi.Domain.Dtos;
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
        Task AddAsync(string name,string userId, CancellationToken cancellationToken);
        

        Task DeleteAsync(int id, CancellationToken cancellationToken);
        Task<Project?> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken);
         Task<(List<ProjectWithPackageInfoDto> Items, int TotalCount, int TotalPages)> GetProjectsWithPackageInfo(
             string userId,
             int pageNumber,
             int pageSize,
             CancellationToken cancellationToken);
    }
}
