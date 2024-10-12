using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Blog.Data.Models;

public class Comentario
{
    public int Id { get; set; }

    [Required(ErrorMessage = "A descrição é obrigatória")]
    public string Descricao { get; set; }
    public string AutorId { get; set; }
    public IdentityUser Autor { get; set; }
    public int PostId { get; set; }
    public Post Post { get; set; }
    public DateTime DataPublicacao { get; set; }

}
