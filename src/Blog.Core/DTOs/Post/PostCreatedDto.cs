using System;
using Blog.Core.DTOs.Autor;

namespace Blog.Core.DTOs.Post;

public class PostCreatedDto
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public DateTime DataPublicacao { get; set; }
    public AutorDto Autor { get; set; }

}
