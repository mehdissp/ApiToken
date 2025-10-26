using JWTApi.Domain.Dtos.Comment;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces;
using JWTApi.Domain.Interfaces.Comments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.Services.Comments
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

        public async Task<List<CommentDto>> GetCommentDtoAsync(int todoId,string userId,CancellationToken cancellationToken)
        {
            await _unitOfWork.CheckAccess(todoId, userId, cancellationToken);
            return await _comment.GetComment(todoId, userId, cancellationToken);
        }

        public async Task InsertComment(string message,string userId,int todoId,CancellationToken cancellationToken)
        {
           var comment = new Comment(todoId ,message, userId);
            await _unitOfWork.CheckAccess(todoId, userId, cancellationToken);
            await _comment.InsertComment(comment, cancellationToken);
            await _unitOfWork.SaveChanges(cancellationToken);

        }

        public async Task DeleteComment(int id,string userId,CancellationToken cancellationToken)
        {
            //await _unitOfWork.CheckAccess(todoId, userId, cancellationToken);
            await _comment.DeleteComment(id, userId, cancellationToken);
            await _unitOfWork.SaveChanges(cancellationToken);
        }
        public async Task UpdateSeenFlag(int todoId,string userId,CancellationToken cancellationToken)
        {
            await _unitOfWork.CheckAccess(todoId, userId, cancellationToken);
            await _comment.UpdateSeenFlag(todoId, userId, cancellationToken);
            await _unitOfWork.SaveChanges(cancellationToken);
        }

    }
}
