using Blog.Data.Data;
using Blog.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Blog.Api.DTOs.Posts;
using Blog.Api.DTOs.Autor;
using Blog.Api.DTOs.Comentarios;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Blog.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/posts")]

public class PostsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PostsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Listar todos os posts", Description = "Esse endpoint retorna todos os posts cadastrados.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<IEnumerable<PostListDto>>> GetPosts(int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            return BadRequest("PageNumber e PageSize precisam ser maior que 0.");
        }

        var posts = await _context.Posts
         .Include(p => p.Autor)
         .Skip((pageNumber - 1) * pageSize)
         .Take(pageSize)
         .ToListAsync();

        var postListDto = posts.Select(post => new PostListDto
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
        }).ToList();

        return Ok(postListDto);
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Lista post por id", Description = "Esse endpoint retorna um post com comentários filtrado por id.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<PostDetailDto>> GetPost(int id)
    {
        if (_context.Posts == null)
        {
            return NotFound();
        }

        var post = await _context.Posts
              .Include(p => p.Autor)
              .Include(c => c.Comentarios).ThenInclude(p => p.Autor)
              .FirstOrDefaultAsync(m => m.Id == id);

        if (post == null)
        {
            return NotFound();
        }

        var postoDetailDto = new PostDetailDto
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
                Comentario = comment.Descricao,
                Autor = new AutorDto
                {
                    Id = comment.Autor.Id,
                    Email = comment.Autor.Email
                },
                DataPublicacao = comment.DataPublicacao
            }).ToList()
        };


        return Ok(postoDetailDto);
    }
    [HttpPost]
    [SwaggerOperation(Summary = "Criar um post", Description = "Esse endpoint cria um post.")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<PostListDto>> CreatePost(PostCreateUpdateDto postCreateUpdateDto)
    {
        if (_context.Posts == null)
        {
            return Problem("Erro ao criar um produto, contate o suporte!");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(new ValidationProblemDetails(ModelState)
            {
                Title = "Um ou mais erros de validação ocorreram!"
            });
        }
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var post = new Post
        {
            Descricao = postCreateUpdateDto.Descricao,
            AutorId = userId,
            Titulo = postCreateUpdateDto.Titulo,
            DataPublicacao = DateTime.Now
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();
        post = await _context.Posts
            .Include(p => p.Autor)
             .FirstOrDefaultAsync(p => p.Id == post.Id);

        var postListDto = new PostListDto
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
        return CreatedAtAction(nameof(GetPost), new { id = postListDto.Id }, postListDto);
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Editar um post", Description = "Esse endpoint edita um post.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> PutPost(int id, PostCreateUpdateDto postCreateUpdateDto)
    {
        var post = await _context.Posts.FindAsync(id);

        if (id != post.Id) return BadRequest();
        _context.Entry(post).State = EntityState.Modified;

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (post.AutorId != userId && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        post.Titulo = postCreateUpdateDto.Titulo;
        post.Descricao = postCreateUpdateDto.Descricao;

        try
        {
            await _context.SaveChangesAsync();
        }
        // Tratar exceções de concorrência (2 usuários tentando modificar mesmo produto)
        catch (DbUpdateConcurrencyException)
        {
            if (!PostExists(id))
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
    [SwaggerOperation(Summary = "Deletar um post", Description = "Esse endpoint deleta um post.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> DeletePost(int id)
    {
        if (_context.Posts == null)
        {
            return NotFound();
        }

        var post = await _context.Posts.FindAsync(id);

        if (post == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (post.AutorId != userId && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id:int}/comentarios")]
    [SwaggerOperation(Summary = "Incluir um comentário em um post", Description = "Esse endpoint cria um comentário vinculado a um post.")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> AddCommentOnPost(int id, CommentCreateUpdateDto CommentCreateUpdateDto)
    {
        var post = await _context.Posts.FindAsync(id);

        if (ModelState.IsValid)
        {

            if (post == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            var comentario = new Comentario
            {
                Descricao = CommentCreateUpdateDto.Comentario,
                PostId = id,
                AutorId = userId,
                DataPublicacao = DateTime.Now
            };

            _context.Comentarios.Add(comentario);
            await _context.SaveChangesAsync();
        }
        return Created();

    }
    private bool PostExists(int id)
    {
        return (_context.Posts?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}

