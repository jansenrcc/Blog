using System;
using Blog.Core.DTOs.Comentario;
using Blog.Core.DTOs.Post;

namespace Blog.Core.Interfaces;

public interface IPostService
{
    Task<IEnumerable<PostListDto>> GetAllPostsAsync(int pageNumber, int pageSize);
    Task<PostDetailDto> GetPostAsync(int id);
    Task<PostCreatedDto> CreatePostAsync(PostCreateUpdateDto postCreateDto, string userId);
    Task UpdatePostAsync(PostCreateUpdateDto postUpdateDto, string userId, int postId);
    Task DeletePostAsync(int postId);
    Task<CommentDetailDto> AddCommentOnPostAsync(int postId, string userId, CommentCreateUpdateDto commentCreateDto);
    Task<bool> IsAuthorAsync(int postId, string userId);


}
