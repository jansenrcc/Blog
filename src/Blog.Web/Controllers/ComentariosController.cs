using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Blog.Core.Interfaces;
using Blog.Core.DTOs.Comentario;


namespace Blog.Web.Controllers
{
    [Authorize]
    public class ComentariosController : Controller
    {

        private readonly ICommentService _commentService;
        private readonly ILogger<ComentariosController> _logger;

        public ComentariosController(ICommentService commentService, ILogger<ComentariosController> logger)
        {

            _commentService = commentService;
            _logger = logger;
        }


        public async Task<IActionResult> Edit(int id)
        {
            var model = await _commentService.GetCommentAsync(id);
            if (model == null)
            {
                return NotFound("Comentário não encontrado.");
            }
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CommentCreateUpdateDto commentUpdateDto, int postId)
        {
            if (!ModelState.IsValid)
            {
                return View(commentUpdateDto);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID não encontrado ao tentar editar o comentário.");
                return Unauthorized("Usuário não autenticado.");
            }

            try
            {
                await _commentService.UpdateCommentAsync(commentUpdateDto, userId, id);
                return RedirectToAction("Details", "Posts", new { id = postId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar o comentário com ID {CommentId} para o post {PostId}.", id, postId);
                return View("Error");
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var model = await _commentService.GetCommentAsync(id);
            if (model == null)
            {
                return NotFound("Comentário não encontrado.");
            }
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int postId, string authorId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
       
            var result = await _commentService.GetCommentAsync(id);
            if (result == null)
            {
                return NotFound("Comentário não encontrado para exclusão.");
            }

            if (!isAdmin && authorId != userId)
            {
                return Forbid();
            }

            try
            {
                await _commentService.DeleteCommentAsync(id);
                return RedirectToAction("Details", "Posts", new { id = postId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir o comentário com ID {CommentId} para o post {PostId}.", id, postId);
                return View("Error");
            }
        }
    }
}