using Blog.Core.DTOs.Comentario;
using Blog.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Blog.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/comentarios")]

public class ComentariosController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly ILogger<ComentariosController> _logger;

    public ComentariosController(ICommentService commentService, ILogger<ComentariosController> logger)
    {

        _commentService = commentService;
        _logger = logger;
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Editar um comentário", Description = "Esse endpoint edita um comentário.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> PutComment(int id, CommentCreateUpdateDto commentUpdateDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        try
        {
            if (await _commentService.IsAuthorAsync(id, userId) || isAdmin)
            {
                await _commentService.UpdateCommentAsync(commentUpdateDto, userId, id);
                return NoContent();
            }

            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar o comentário.");
            return BadRequest("Ocorreu um erro ao processar a solicitação");
        }
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Deletar um comentário", Description = "Esse endpoint deleta um comentário.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        try
        {
            if (await _commentService.IsAuthorAsync(id, userId) || isAdmin)
            {
                await _commentService.DeleteCommentAsync(id);
                return NoContent();
            }
            return Forbid();
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar o comentário.");
            return BadRequest("Ocorreu um erro ao processar a solicitação");
        }
    }
}
