using System;
using Blog.Core.DTOs.Comentario;

namespace Blog.Core.Interfaces;

public interface ICommentService
{
    Task<CommentDetailDto> GetCommentAsync(int id);
    Task UpdateCommentAsync(CommentCreateUpdateDto commentUpdateDto, string userId, int commentId);
    Task DeleteCommentAsync(int id);
    Task<bool> IsAuthorAsync(int commentId, string userId);

}
