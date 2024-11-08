using System;
using Microsoft.AspNetCore.Identity;

namespace Blog.Data.Models;

public class Autor
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Nome { get; set; }

    //Propriedade de navegação.
    public IdentityUser User { get; set; }

}
