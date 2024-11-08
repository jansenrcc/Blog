using System;
using Blog.Core.DTOs.Autor;
using Blog.Core.DTOs.Comentario;

namespace Blog.Core.DTOs.Post;

public class PostDetailDto
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public DateTime DataPublicacao { get; set; }
    public AutorDto Autor { get; set; }
    public List<CommentDetailDto> Comentarios { get; set; } = new List<CommentDetailDto>();

}
