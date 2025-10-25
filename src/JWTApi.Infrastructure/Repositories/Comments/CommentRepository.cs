using JWTApi.Domain.Dtos.Comment;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces.Comments;
using JWTApi.Infrastructure.Data;
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
        public async Task<List<CommentDto>> GetComment(int todoId, CancellationToken cancellationToken)
        {
        return await _context.Comments
            .Where(t => t.TodoId == todoId).Select(t => new CommentDto
            {
                Id = t.Id,
                CreatedAt = t.CreatedAt,
                Message = t.Message,
                TodoId = t.TodoId,
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
    }
}
