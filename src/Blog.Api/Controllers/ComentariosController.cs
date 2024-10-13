using Blog.Api.DTOs.Comentarios;
using Blog.Data.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Blog.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/comentarios")]

public class ComentariosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ComentariosController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Editar um comentário", Description = "Esse endpoint edita um comentário.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> PutComment(int id, CommentCreateUpdateDto commentCreateUpdateDto)
    {
        var comentario = await _context.Comentarios.FindAsync(id);

        if (id != comentario.Id) return BadRequest();
        _context.Entry(comentario).State = EntityState.Modified;

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (comentario.AutorId != userId && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        comentario.Descricao = commentCreateUpdateDto.Comentario;

        try
        {
            await _context.SaveChangesAsync();
        }
        // Tratar exceções de concorrência (2 usuários tentando modificar mesmo comentário)
        catch (DbUpdateConcurrencyException)
        {
            if (!CommentExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Deletar um comentário", Description = "Esse endpoint deleta um comentário.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> DeleteComment(int id)
    {
        if (_context.Posts == null)
        {
            return NotFound();
        }

        var comentario = await _context.Comentarios.FindAsync(id);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (comentario.AutorId != userId && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        if (comentario == null)
        {
            return NotFound();
        }

        _context.Comentarios.Remove(comentario);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CommentExists(int id)
    {
        return (_context.Posts?.Any(e => e.Id == id)).GetValueOrDefault();
    }

}
