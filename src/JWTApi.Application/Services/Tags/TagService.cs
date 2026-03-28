using JWTApi.Application.DTOs.Tags;
using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.TagProjects;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces;
using JWTApi.Domain.Interfaces.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.Services.Tags
{
   public class TagService
    {
        private readonly IUnitOfWork _unit;
        private readonly ITagRepository  _tagRepository;
        public TagService(ITagRepository tagRepository, IUnitOfWork unit)
        {
            _tagRepository = tagRepository;
            _unit = unit;
        }
        public async Task<(List<Tag> Tags, int TotalCount, int TotalPage)> GetTagsAsync(string userId,int pageNumber,int pageSize,string keyValue, CancellationToken cancellationToken)
        {
            return await _tagRepository.GetTagsWithPaging(StringToGuidConverter.ConvertToGuid(userId), pageNumber, pageSize, keyValue, cancellationToken);
        }

        public async Task InsertTag(TagItemDtos tagDtos,string userId,CancellationToken cancellationToken)
        {
            var tag = new Tag(tagDtos.Name, tagDtos.Desc, tagDtos.Color, StringToGuidConverter.ConvertToGuid(userId));
            await _tagRepository.InsertTag(tag, cancellationToken);
            await _unit.SaveChanges(cancellationToken);
        }

        public async Task UpdateTags(TagItemDtos tagDtos,string userId,CancellationToken cancellationToken)
        {
            var tag = await _tagRepository.GetTagById((int)tagDtos.Id, cancellationToken);
            tag.UpdateTag(tagDtos.Name, tagDtos.Desc, tagDtos.Color);
            await _unit.SaveChanges(cancellationToken);
        }
        public async Task DeleteTags(int id,string userId,CancellationToken cancellationToken)
        {
            await _tagRepository.DeleteTag(id, cancellationToken);
            await _unit.SaveChanges(cancellationToken);
        }


        public async Task<PagedResult<TagProjectDtos>> GetTagProjectDtos(string userId, int projectId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            return await _tagRepository.GetTagProjectDtos(userId, projectId, pageNumber, pageSize, cancellationToken);
        }

    }
}
