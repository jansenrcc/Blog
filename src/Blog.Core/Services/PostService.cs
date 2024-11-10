using Blog.Core.DTOs.Autor;
using Blog.Core.DTOs.Comentario;
using Blog.Core.DTOs.Post;
using Blog.Core.Interfaces;
using Blog.Data.Data;
using Blog.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Blog.Core.Services;

public class PostService : IPostService
{
    private readonly ILogger<PostService> _logger;
    private readonly ApplicationDbContext _context;

    public PostService(ApplicationDbContext context, ILogger<PostService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<PostListDto>> GetAllPostsAsync(int pageNumber, int pageSize)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            throw new ArgumentOutOfRangeException("Parâmetros de paginação devem ser maiores que zero.");
        }
        try
        {
            var posts = await _context.Posts
            .Include(p => p.Autor)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            return posts.Select(post => new PostListDto
            {
                Id = post.Id,
                Titulo = post.Titulo,
                Descricao = post.Descricao,
                DataPublicacao = post.DataPublicacao,
                Autor = new AutorDto
                {
                    Id = post.Autor.Id,
                    Email = post.Autor.Email
                }
            });
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter a lista de posts.");
            throw;
        }

    }

    public async Task<PostDetailDto> GetPostAsync(int id)
    {
        var post = await _context.Posts
               .Include(p => p.Autor)
               .Include(c => c.Comentarios).ThenInclude(p => p.Autor)
               .FirstOrDefaultAsync(m => m.Id == id)
               ?? throw new KeyNotFoundException("Post não encontrado.");

        return new PostDetailDto
        {
            Id = post.Id,
            Titulo = post.Titulo,
            Descricao = post.Descricao,
            DataPublicacao = post.DataPublicacao,
            Autor = new AutorDto
            {
                Id = post.Autor.Id,
                Email = post.Autor.Email
            },
            Comentarios = post.Comentarios.Select(comment => new CommentDetailDto
            {
                Id = comment.Id,
                PostId = comment.PostId,
                Comentario = comment.Descricao,
                Autor = new AutorDto
                {
                    Id = comment.Autor.Id,
                    Email = comment.Autor.Email
                },
                DataPublicacao = comment.DataPublicacao
            }).ToList()

        };
    }

    public async Task<PostCreatedDto> CreatePostAsync(PostCreateUpdateDto postCreateDto, string userId)
    {

        var post = new Post
        {
            Descricao = postCreateDto.Descricao,
            AutorId = userId,
            Titulo = postCreateDto.Titulo,
            DataPublicacao = DateTime.Now
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();
        post = await _context.Posts
            .Include(p => p.Autor)
            .FirstOrDefaultAsync(p => p.Id == post.Id);

        return new PostCreatedDto
        {
            Id = post.Id,
            Titulo = post.Titulo,
            Descricao = post.Descricao,
            DataPublicacao = post.DataPublicacao,
            Autor = new AutorDto
            {
                Id = post.Autor.Id,
                Email = post.Autor.Email
            }

        };
    }

    public async Task UpdatePostAsync(PostCreateUpdateDto postUpdateDto, string userId, int postId)
    {
        var post = await _context.Posts.FindAsync(postId)
        ?? throw new KeyNotFoundException("Post não encontrado.");

        _context.Entry(post).State = EntityState.Modified;

        post.Titulo = postUpdateDto.Titulo;
        post.Descricao = postUpdateDto.Descricao;
        post.AutorId = userId;

        await _context.SaveChangesAsync();

    }

    public async Task DeletePostAsync(int postId)
    {
        var post = await _context.Posts.FindAsync(postId)
         ?? throw new KeyNotFoundException("Post não encontrado para exclusão.");

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();
    }

    public async Task<CommentDetailDto> AddCommentOnPostAsync(int postId, string userId, CommentCreateUpdateDto commentCreateDto)
    {
        var post = await _context.Posts.FindAsync(postId)
            ?? throw new KeyNotFoundException("Post não encontrado.");

        var autor = await _context.Autores.FirstOrDefaultAsync(a => a.Id == userId)
            ?? throw new KeyNotFoundException("Autor não encontrado.");

        var comentario = new Comentario
        {
            Descricao = commentCreateDto.Comentario,
            PostId = postId,
            AutorId = userId,
            DataPublicacao = DateTime.Now
        };

        _context.Comentarios.Add(comentario);
        await _context.SaveChangesAsync();
        comentario = await _context.Comentarios.FirstOrDefaultAsync(c => c.Id == comentario.Id);

        return new CommentDetailDto
        {
            Id = comentario.Id,
            Comentario = comentario.Descricao,
            Autor = new AutorDto
            {
                Id = comentario.Autor.Id,
                Email = comentario.Autor.Email
            },
            DataPublicacao = comentario.DataPublicacao
        };
    }
    public async Task<bool> IsAuthorAsync(int postId, string userId)
    {
        var post = await _context.Posts.FindAsync(postId);
        if (post == null)
        {
            throw new KeyNotFoundException("Post não encontrado.");
        }

        return post.AutorId == userId;
    }
}


