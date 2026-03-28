using JWTApi.Domain.Dtos.TagProjects;
using JWTApi.Domain.Dtos;
using JWTApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Interfaces.Tags
{
   public interface ITagRepository
    {
        Task<(List<Tag> Tags, int TotalCount, int TotalPage)> GetTagsWithPaging(Guid userId, int pageNumber, int pageSize, string? keyValue, CancellationToken cancellationToken);
        Task<Tag> GetTagById(int id, CancellationToken cancellationToken);
        Task InsertTag(Tag tag, CancellationToken cancellationToken);

        Task DeleteTag(int id, CancellationToken cancellationToken);
        Task<PagedResult<TagProjectDtos>> GetTagProjectDtos(string userId, int projectId, int pageNumber, int pageSize, CancellationToken cancellationToken);


    }
}
