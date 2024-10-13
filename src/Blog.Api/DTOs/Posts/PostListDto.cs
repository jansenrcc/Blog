using System;
using Blog.Api.DTOs.Autor;

namespace Blog.Api.DTOs.Posts;

public class PostListDto
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public DateTime DataPublicacao { get; set; }
    public AutorDto Autor { get; set; }

}
