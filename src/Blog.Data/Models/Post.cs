using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Blog.Data.Models;

public class Post
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public DateTime DataPublicacao { get; set; }

    // FKs
    public string AutorId { get; set; }

    //Propriedades de navegação.
    public Autor Autor { get; set; }
    public ICollection<Comentario> Comentarios { get; set; }

}
