using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using Blog.Core.DTOs.Post;
using Blog.Core.Interfaces;
using Blog.Core.DTOs.Comentario;

namespace Blog.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/posts")]

public class PostsController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly ILogger<PostsController> _logger;

    public PostsController(IPostService postService, ILogger<PostsController> logger)
    {
        _postService = postService;
        _logger = logger;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Listar todos os posts", Description = "Esse endpoint retorna todos os posts cadastrados.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<IEnumerable<PostListDto>>> GetPosts(int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            var result = await _postService.GetAllPostsAsync(pageNumber, pageSize);
            if (result == null || !result.Any())
            {
                return NotFound("Nenhum post encontrado.");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter a lista de posts");
            return BadRequest("Ocorreu um erro ao processar a solicitação.");
        }
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Lista post por id", Description = "Esse endpoint retorna um post com comentários filtrado por id.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<PostDetailDto>> GetPost(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Tentativa de acesso com um ID inválido: {Id}", id);
            return BadRequest("ID inválido.");
        }
        try
        {
            var result = await _postService.GetPostAsync(id);
            if (result == null)
            {
                _logger.LogInformation("Post com ID {Id} não encontrado.", id);
                return NotFound("O post solicitado não foi encontrado.");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao tentar obter detalhes do post com ID {Id}.", id);
            return BadRequest("Ocorreu um erro ao processar a solicitação.");
        }
    }
    [HttpPost]
    [SwaggerOperation(Summary = "Criar um post", Description = "Esse endpoint cria um post.")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<PostListDto>> CreatePost(PostCreateUpdateDto postCreateDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("User ID não encontrado ao tentar criar um post.");
            return Unauthorized("Usuário não autenticado.");
        }
        if (!ModelState.IsValid)
        {
            return BadRequest("Dados inválidos para criação do post.");
        }
        try
        {
            var model = await _postService.CreatePostAsync(postCreateDto, userId);
            return CreatedAtAction(nameof(GetPost), new { id = model.Id }, model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar o post.");
            return BadRequest("Ocorreu um erro ao processar a solicitação");
        }
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Editar um post", Description = "Esse endpoint edita um post.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> PutPost(int id, PostCreateUpdateDto postUpdateDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        try
        {
            if (await _postService.IsAuthorAsync(id, userId) || isAdmin)
            {
                await _postService.UpdatePostAsync(postUpdateDto, userId, id);
                return NoContent();
            }
           return Forbid();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar o post.");
            return BadRequest("Ocorreu um erro ao processar a solicitação");
        }
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Deletar um post", Description = "Esse endpoint deleta um post.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> DeletePost(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        try
        {
            if (await _postService.IsAuthorAsync(id, userId) || isAdmin)
            {
                await _postService.DeletePostAsync(id);
                return NoContent();
            }

            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar o post.");
            return BadRequest("Ocorreu um erro ao processar a solicitação");
        }
    }

    [HttpPost("{id:int}/comentarios")]
    [SwaggerOperation(Summary = "Incluir um comentário em um post", Description = "Esse endpoint cria um comentário vinculado a um post.")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<CommentDetailDto>> AddCommentOnPost(int id, CommentCreateUpdateDto commentCreateDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("User ID não encontrado ao tentar adicionar um comentário.");
            return Unauthorized("Usuário não autenticado.");
        }
        if (!ModelState.IsValid)
        {
            return BadRequest("Dados inválidos para adição de comentário ao post.");
        }

        try
        {
            var result = await _postService.AddCommentOnPostAsync(id, userId, commentCreateDto);
            if (result == null)
            {
                return NotFound("Post não encontrado para adicionar o comentário.");
            }
            return RedirectToAction("GetPost", new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar comentário ao post.");
            return BadRequest("Ocorreu um erro ao processar a solicitação");
        }


    }
}

