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

        public async Task InsertProject(string name, string userId, , CancellationToken cancellationToken)
        {

             await _projectRepository.AddAsync(name, userId, cancellationToken);
        }
    }
}
