using System;
using Blog.Core.DTOs.Autor;

namespace Blog.Core.DTOs.Comentario;

public class CommentDetailDto
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string Comentario { get; set; }
    public AutorDto Autor { get; set; }
    public DateTime DataPublicacao { get; set; }

}
