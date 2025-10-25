using JWTApi.Domain.Dtos.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Interfaces.Comment
{
   public interface IComment
    {
        Task<List<CommentDto>> GetComment(int todoId, CancellationToken cancellationToken);
    }
}
