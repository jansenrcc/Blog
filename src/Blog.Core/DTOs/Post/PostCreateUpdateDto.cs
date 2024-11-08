using System;
using System.ComponentModel.DataAnnotations;

namespace Blog.Core.DTOs.Post;

public class PostCreateUpdateDto
{
    [Required(ErrorMessage = "O título é obrigatório")]
    public string Titulo { get; set; }

    [Required(ErrorMessage = "A descrição é obrigatória")]
    public string Descricao { get; set; }

}
