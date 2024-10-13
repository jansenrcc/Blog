using System;
using Blog.Api.DTOs.Autor;
using Blog.Api.DTOs.Comentarios;

namespace Blog.Api.DTOs.Posts;

public class PostDetailDto
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public DateTime DataPublicacao { get; set; }
    public AutorDto Autor { get; set; }
     public List<CommentDetailDto> Comentarios { get; set; } = new List<CommentDetailDto>();
}
