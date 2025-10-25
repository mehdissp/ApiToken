using JWTApi.Domain.Dtos.Comment;
using JWTApi.Domain.Interfaces;
using JWTApi.Domain.Interfaces.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.Services.Comment
{
   public class CommentService
    {
        private IComment _comment;
        private IUnitOfWork _unitOfWork;
        public CommentService(IComment comment, IUnitOfWork unitOfWork)
        {
            _comment = comment;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CommentDto>> GetCommentDtoAsync(int todoId,CancellationToken cancellationToken)
        {
            return await _comment.GetComment(todoId, cancellationToken);
        }

    }
}
