using System;
using System.ComponentModel.DataAnnotations;

namespace Blog.Api.DTOs.Posts;

public class PostCreateUpdateDto
{
    [Required(ErrorMessage = "O título é obrigatório")]
    public string Titulo { get; set; }

    [Required(ErrorMessage = "A descrição é obrigatória")]
    public string Descricao { get; set; }

}
