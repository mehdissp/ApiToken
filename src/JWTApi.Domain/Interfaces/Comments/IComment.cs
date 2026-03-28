using JWTApi.Domain.Dtos.Comment;
using JWTApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Interfaces.Comments
{
   public interface IComment
    {
        Task<List<CommentDto>> GetComment(int todoId, string userId, CancellationToken cancellationToken);
        Task InsertComment(Comment comment, CancellationToken cancellationToken);
        Task<Comment?> GetCommentFindByIdAsync(int id, CancellationToken cancellationToken);
        Task DeleteComment(int id, string userId, CancellationToken cancellationToken);
        Task UpdateSeenFlag(int todoId, string userId, CancellationToken cancellationToken);
    }
}
