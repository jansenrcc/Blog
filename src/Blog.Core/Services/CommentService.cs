using Blog.Core.DTOs.Autor;
using Blog.Core.DTOs.Comentario;
using Blog.Core.Interfaces;
using Blog.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace Blog.Core.Services;

public class CommentService : ICommentService
{
    private readonly ApplicationDbContext _context;

    public CommentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CommentDetailDto> GetCommentAsync(int id)
    {
        var comentario = await _context.Comentarios
            .Include(p => p.Autor)
            .FirstOrDefaultAsync(c => c.Id == id);

        return new CommentDetailDto
        {
            Id = comentario.Id,
            PostId = comentario.PostId,
            Comentario = comentario.Descricao,
            Autor = new AutorDto
            {
                Id = comentario.Autor.Id,
                Email = comentario.Autor.Email
            },
            DataPublicacao = comentario.DataPublicacao
        };

    }

    public async Task UpdateCommentAsync(CommentCreateUpdateDto commentUpdateDto, string userId, int commentId)
    {
        var comentario = await _context.Comentarios.FindAsync(commentId);
        _context.Entry(comentario).State = EntityState.Modified;

        comentario.Descricao = commentUpdateDto.Comentario;
        comentario.AutorId = userId;

        await _context.SaveChangesAsync();

    }

    public async Task DeleteCommentAsync(int id)
    {
        var comentario = await _context.Comentarios.FindAsync(id);
        _context.Comentarios.Remove(comentario);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsAuthorAsync(int commentId, string userId)
    {
        var comentario = await _context.Comentarios.FindAsync(commentId);
        if (comentario == null)
        {
            throw new KeyNotFoundException("Comentário não encontrado.");
        }

        return comentario.AutorId == userId;
    }

}
