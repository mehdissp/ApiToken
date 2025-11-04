using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.ProjectUsers;
using JWTApi.Domain.Dtos.TagProjects;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces.Tags;
using JWTApi.Infrastructure.Data;
using JWTApi.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Repositories.Tags
{
    public class TagRepository : ITagRepository
    {
        private readonly AppDbContext _context;
        public TagRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task DeleteTag(int id, CancellationToken cancellationToken)
        {
            var tag = await GetTagById(id, cancellationToken);
            if (await CheckTagTodo(id,cancellationToken))
            {
                throw new RestBasedException(ApiErrorCodeMessage.Error_Refrence);
            }
            else
            {
                tag.IsDeleted = true;
            }
            
        }

        public async Task<(List<Tag> Tags, int TotalCount,int TotalPage)> GetTagsWithPaging(Guid userId, int pageNumber, int pageSize,string? keyValue, CancellationToken cancellationToken)
        {
            try
            {

                var query = _context.Tags
          .Include(s => s.TagProjects)
          .Where(s => s.UserId == userId && s.IsDeleted == false);

                if (!string.IsNullOrEmpty(keyValue))
                {
                    query = query.Where(s => s.Name.Contains(keyValue) ||s.DescriptionRows.Contains(keyValue));
                }


                var totalCount = await query.CountAsync(cancellationToken);

  
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var tags = await query
                    .OrderBy(s => s.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                return (tags, totalCount, totalPages);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Tag> GetTagById(int id, CancellationToken cancellationToken)
        {
            return await _context.Tags.FindAsync(id, cancellationToken);
        }

        public async Task InsertTag(Tag tag, CancellationToken cancellationToken)
        {
            await _context.Tags.AddAsync(tag, cancellationToken);
        }
        private async Task<bool> CheckTagTodo(int tagId,CancellationToken cancellationToken)
        {
            return await _context.TodoTags.AnyAsync(s => s.TagId == tagId, cancellationToken);
        }



        public async Task<PagedResult<TagProjectDtos>> GetTagProjectDtos(string userId, int projectId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            //var baseQuery = from u in _context.Tags
            //                join r in _context.TagProjects on u.Id equals r.TagId into projectTags
            //                from ur in projectTags.DefaultIfEmpty()
            //                join q in _context.Projects on ur.ProjectId equals q.Id into projects
            //                from role in projects.DefaultIfEmpty()
            //                where (u.UserId.ToString() == GetUserIdManager(userId) || u.UserId.ToString()== userId)
            //                && u.Id.ToString() != userId
            //                select new TagProjectDtos
            //                {
            //                    Id = u.Id,
            //                    TagName = u.Name,
            //                    Color = u.Color,
            //                    IsCheck = _context.TagProjects.Any(pu => pu.TagId == u.Id && pu.ProjectId == projectId)
            //                };
            var userGuid = Guid.Parse(userId);
            var managerGuid = Guid.Parse(GetUserIdManager(userId));

            var baseQuery = _context.Tags
                .Where(u => (u.UserId == userGuid || u.UserId == managerGuid ) && u.IsDeleted==false
                           )
                .Select(u => new TagProjectDtos
                {
                    Id = u.Id,
                    TagName = u.Name,
                    Color = u.Color,
                    IsCheck = u.TagProjects.Any(pu => pu.ProjectId == projectId)
                })
                .Distinct();

            // گرفتن تعداد کل رکوردها
            var totalCount = await baseQuery.CountAsync(cancellationToken);

            // اعمال صفحه‌بندی
            var users = await baseQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<TagProjectDtos>
            {
                Items = users,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        private string GetUserIdManager(string userId)
        {

            var baseQuery = from u in _context.Users
                            join ur in _context.UserRoles on u.UserId equals ur.UserId into userRoles
                            from ur in userRoles.DefaultIfEmpty()
                            join r in _context.Roles on ur.RoleId equals r.Id into roles
                            from role in roles.DefaultIfEmpty()
                            where u.Id == Guid.Parse(userId) // فقط کاربرانی که نقش دارند
                            select new
                            {
                                User = u,
                                RoleId = ur.RoleId,
                                RoleName = role.Name,
                                Role = role
                            };
            var baseQuery2 = (from u in _context.Users
                              join ur in _context.UserRoles on u.Id equals ur.UserId
                              join r in _context.Roles on ur.RoleId equals r.Id
                              where u.Id == baseQuery.FirstOrDefault().User.UserId
                              select new
                              {
                                  User = u,
                                  RoleId = ur.RoleId,
                                  RoleName = r.Name,
                                  Role = r
                              }).ToList();
            if (baseQuery2.Count() == 0)
            {
                return userId;
            }
            if (baseQuery2.FirstOrDefault().Role.TypeRole == 1)
            {
                var t = baseQuery2.First().User.Id.ToString();
                return t;
              
            }
            else
            {
                return userId;
            }

        }




    }
}
