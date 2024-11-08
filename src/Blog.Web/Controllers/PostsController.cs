using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Blog.Core.Interfaces;
using Blog.Core.DTOs.Post;
using Blog.Core.DTOs.Comentario;

namespace Blog.Web.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {

        private readonly IPostService _postService;
        private readonly ILogger<PostsController> _logger;

        public PostsController(IPostService postService, ILogger<PostsController> logger)
        {
            _postService = postService;
            _logger = logger;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var model = await _postService.GetAllPostsAsync(pageNumber, pageSize);
                if (model == null || !model.Any())
                {
                    ViewBag.Message = "Nenhum post encontrado.";
                }
                return View(model);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter a lista de posts"); 
                return View("Error");
            }
        }
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Tentativa de acesso com um ID inválido: {Id}", id);
                return BadRequest("ID inválido.");
            }
            try
            {
                var model = await _postService.GetPostAsync(id);
                if (model == null)
                {
                    _logger.LogInformation("Post com ID {Id} não encontrado.", id);
                    return NotFound("O post solicitado não foi encontrado.");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar obter detalhes do post com ID {Id}.", id);
                return View("Error");
            }
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostCreateUpdateDto postCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return View(postCreateDto);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID não encontrado ao tentar criar um post.");
                return Unauthorized("Usuário não autenticado.");
            }

            try
            {
                var model = await _postService.CreatePostAsync(postCreateDto, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar o post.");
                return View("Error");
            }
        }

        public async Task<IActionResult> Edit(int id)
        {

            var model = await _postService.GetPostAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PostCreateUpdateDto postUpdateDto, string authorId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && authorId != userId)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return View(postUpdateDto);
            }

            try
            {
                await _postService.UpdatePostAsync(postUpdateDto, userId, id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar o post com ID {Id}.", id);
                return View("Error");
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var model = await _postService.GetPostAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string authorId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && authorId != userId)
            {
                return Forbid();
            }

            try
            {
                await _postService.DeletePostAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir o post com ID {Id}.", id);
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int id, CommentCreateUpdateDto commentCreateDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID não encontrado ao tentar adicionar um comentário.");
                return Unauthorized("Usuário não autenticado.");
            }

            if (!ModelState.IsValid)
            {
                return View(commentCreateDto);
            }

            try
            {
                var result = await _postService.AddCommentOnPostAsync(id, userId, commentCreateDto);
                if (result == null)
                {
                    return NotFound("Post não encontrado para adicionar o comentário.");
                }
                return RedirectToAction("Details", new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar comentário ao post com ID {Id}.", id);
                return View("Error");
            }

        }

    }

}

