using System;
using System.ComponentModel.DataAnnotations;

namespace Blog.Web.ViewModels;

public class ComentarioViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "A descrição é obrigatória")]
    public string Descricao { get; set; }
    public string AutorId { get; set; }
    public int PostId { get; set; }   
    public DateTime DataPublicacao { get; set; }
}
