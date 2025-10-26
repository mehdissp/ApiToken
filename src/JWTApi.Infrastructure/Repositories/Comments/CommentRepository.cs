using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.Comment;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces.Comments;
using JWTApi.Infrastructure.Data;
using JWTApi.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Repositories.Comments
{
    public class CommentRepository : IComment
    {

        private readonly AppDbContext _context;
        public CommentRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<CommentDto>> GetComment(int todoId,string userId, CancellationToken cancellationToken)
        {
        return await _context.Comments
            .Where(t => t.TodoId == todoId && t.IsDeleted==false).Select(t => new CommentDto
            {
                Id = t.Id,
                CreatedAt = t.CreatedAt,
                Message = t.Message,
                TodoId = t.TodoId,
                UserId=t.UserId.ToString(),
                ShowDeleted= t.UserId.ToString()==userId ?true :false,
                SeenAt=t.UserId.ToString() == userId ? t.SeenAt :null,
                UserAuthor = t.UserId != null ?
                        _context.Users
                            .Where(u => u.Id.ToString() == t.UserId.ToString())
                            .Select(u => u.FullName)
                            .First() ?? "نامشخص" : null
            }).ToListAsync(cancellationToken);
        }

        public async Task InsertComment(Comment comment,CancellationToken cancellationToken)
        {
            await _context.Comments.AddAsync(comment, cancellationToken);
        }
        public async Task<Comment?> GetCommentFindByIdAsync(int id,CancellationToken cancellationToken)
        {
            return await _context.Comments.FindAsync(id, cancellationToken);
        }

        public async Task DeleteComment(int id,string userId,CancellationToken cancellationToken)
        {

            var comment = await GetCommentFindByIdAsync(id, cancellationToken);
            if (comment.UserId.ToString()== userId)
            {
                comment.IsDeleted = true;
            }
            else
            {
                throw new RestBasedException(ApiErrorCodeMessage.Error_Access);
            }
        }

        public async Task UpdateSeenFlag(int todoId,string userId,CancellationToken cancellationToken)
        {
            var userGuid = Guid.Parse(userId);

            await _context.Comments
                .Where(c => c.TodoId == todoId && c.UserId != userGuid && c.SeenAt ==null )
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(c => c.IsSeen, true).SetProperty(s=>s.SeenAt,DateTime.Now),
                cancellationToken);

        }
        

    }
}
