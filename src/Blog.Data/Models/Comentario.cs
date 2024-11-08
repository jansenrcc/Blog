using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Blog.Data.Models;

public class Comentario
{
    public int Id { get; set; }
    public string Descricao { get; set; }
    public DateTime DataPublicacao { get; set; }

    // FKs
    public string AutorId { get; set; }
    public int PostId { get; set; }

    //Propriedades de navegação.
    public Autor Autor { get; set; }
    public Post Post { get; set; }


}
