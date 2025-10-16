using JWTApi.Domain.Dtos;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.Services
{
   public class ProjectService
    {
        private readonly IProjectRepository _projectRepository;
        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task InsertProject(string name, string userId, CancellationToken cancellationToken)
        {

             await _projectRepository.AddAsync(name, userId, cancellationToken);
             
        }
        public async Task DeleteProject(int id ,string userId, CancellationToken cancellationToken)
        {
            await _projectRepository.DeleteAsync(id, cancellationToken);
        }

        public async Task<(List<ProjectWithPackageInfoDto> Items, int TotalCount, int TotalPages)> GetProjectsAsync(string userId ,  int pageNumber,
             int pageSize, CancellationToken cancellationToken)
        {
            return await _projectRepository.GetProjectsWithPackageInfo(userId,pageNumber,pageSize, cancellationToken);
        }

    }
}
