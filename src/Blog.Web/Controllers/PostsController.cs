using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Blog.Data.Data;
using Blog.Data.Models;
using Blog.Web.ViewModels;

namespace Blog.Web.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var posts = await _context.Posts.Include(p => p.Autor).ToListAsync();
            var postViewModels = posts.Select(post => new PostViewModel
            {
                Id = post.Id,
                Titulo = post.Titulo,
                Autor = post.Autor,
                DataPublicacao = post.DataPublicacao
            }).ToList();

            return View(postViewModels);
        }
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
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

            var postViewModel = new PostViewModel
            {
                Id = post.Id,
                Descricao = post.Descricao,
                Comentarios = post.Comentarios
            };

            return View(postViewModel);
        }


        public IActionResult Create()
        {           
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostViewModel postViewModel)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var novoPost = new Post
                {

                    Descricao = postViewModel.Descricao,
                    AutorId = userId,
                    Titulo = postViewModel.Titulo,
                    DataPublicacao = DateTime.Now

                };
                _context.Add(novoPost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }            
            return View(postViewModel);
        }
        
        public async Task<IActionResult> Edit(int id)
        {

            var post = await _context.Posts.FindAsync(id);            
            if (post == null)
            {
                return NotFound();
            }

            var postViewModel = new PostViewModel
            {
                Id = post.Id,
                Titulo = post.Titulo,
                Descricao = post.Descricao
            };
            
            return View(postViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PostViewModel postViewModel)
        {
            if (id != postViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var postExistente = await _context.Posts.FindAsync(id);
                if (postExistente == null)
                {
                    return NotFound();
                }
                postExistente.Titulo = postViewModel.Titulo;
                postExistente.Descricao = postViewModel.Descricao;

                try
                {
                    _context.Update(postExistente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(postViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(postViewModel);
        }
       
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Autor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            var postViewModel = new PostViewModel
            {
                Id = post.Id,
                Descricao = post.Descricao,
                Titulo = post.Titulo,
                DataPublicacao = post.DataPublicacao
            };

            return View(postViewModel);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int id, string texto)
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
                    Descricao = texto,
                    PostId = id,
                    AutorId = userId,
                    DataPublicacao = DateTime.Now
                };

                _context.Comentarios.Add(comentario);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Details", new { id = post.Id });

        }
        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }

}

