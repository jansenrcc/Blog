using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Blog.Data.Data;
using Blog.Data.Models;
using Blog.Web.ViewModels;

namespace Blog.Web.Controllers
{
    [Authorize]
    public class ComentariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ComentariosController(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comentario = await _context.Comentarios.FindAsync(id);
            if (comentario == null)
            {
                return NotFound();
            }

            var comentarioViewModel = new ComentarioViewModel
            {
                Id = comentario.Id,
                Descricao = comentario.Descricao,
                DataPublicacao = comentario.DataPublicacao,
                PostId = comentario.PostId

            };

            return View(comentarioViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ComentarioViewModel comentarioViewModel)
        {
            if (id != comentarioViewModel.Id)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (comentarioViewModel.AutorId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var novoComentario = await _context.Comentarios.FindAsync(comentarioViewModel.Id);

                    if (novoComentario == null)
                    {
                        return NotFound();
                    }
                    novoComentario.Descricao = comentarioViewModel.Descricao;
                    novoComentario.AutorId = userId;
                    novoComentario.DataPublicacao = DateTime.Now;

                    _context.Update(novoComentario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComentarioExists(comentarioViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Posts", new { id = comentarioViewModel.PostId });
            }

            return View(comentarioViewModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comentario = await _context.Comentarios
                .Include(c => c.Post)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comentario == null)
            {
                return NotFound();
            }

            var comentarioViewModel = new ComentarioViewModel
            {
                Descricao = comentario.Descricao,
                DataPublicacao = comentario.DataPublicacao
            };

            return View(comentarioViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comentario = await _context.Comentarios.FindAsync(id);
            var postId = comentario.PostId;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (comentario.AutorId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            if (comentario != null)
            {
                _context.Comentarios.Remove(comentario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Posts", new { id = postId });
        }

        private bool ComentarioExists(int id)
        {
            return _context.Comentarios.Any(e => e.Id == id);
        }
    }
}