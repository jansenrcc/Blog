using System;
using Blog.Api.DTOs.Autor;

namespace Blog.Api.DTOs.Comentarios;

public class CommentDetailDto
{
    public int Id { get; set; }
    public string Comentario { get; set; }
    public AutorDto Autor { get; set; }     
    public DateTime DataPublicacao { get; set; }

}
