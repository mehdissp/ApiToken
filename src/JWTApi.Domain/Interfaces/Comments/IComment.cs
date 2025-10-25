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
        Task<List<CommentDto>> GetComment(int todoId, CancellationToken cancellationToken);
        Task InsertComment(Comment comment, CancellationToken cancellationToken);
    }
}
