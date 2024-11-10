using System;
using Blog.Data.Data;
using Blog.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Configuration;

public static class DbMigrationsHelpers
{
    public static void UseDbMigrationHelper (this WebApplication app)
    {
        DbMigrationsHelpers.EnsureSeedData(app).Wait();
    }
    public static async Task EnsureSeedData(WebApplication serviceScope)
    {
        // Criar scope
        var services = serviceScope.Services.CreateScope().ServiceProvider;
        await EnsureSeedData(services);
    }

    public static async Task EnsureSeedData(IServiceProvider serviceProvider)
    {
        // Obter ambiente
        using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (env.IsDevelopment() || env.IsEnvironment("Docker"))
        {
            await context.Database.MigrateAsync();
        }

        await EnsureSeedProducts(context);
    }

    public static async Task EnsureSeedProducts(ApplicationDbContext context)
    {
        if (context.Autores.Any())
            return;

        var userId = Guid.NewGuid().ToString();
        var roleId = Guid.NewGuid().ToString();

        string SenhaTextoUsuario = "SenhaSegura123!";
        var HashSenha = new PasswordHasher<IdentityUser>();
        var usuarioTemp = new IdentityUser();
        string hashedPassword = HashSenha.HashPassword(usuarioTemp, SenhaTextoUsuario);


        await context.Users.AddAsync(new IdentityUser
        {
            Id = userId,
            UserName = "teste@teste.com",
            NormalizedUserName = "TESTE@TESTE.COM",
            Email = "teste@teste.com",
            NormalizedEmail = "TESTE@TESTE.COM",
            AccessFailedCount = 0,
            LockoutEnabled = false,
            PasswordHash = hashedPassword,
            TwoFactorEnabled = false,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString()
        });

        await context.Roles.AddAsync(new IdentityRole
        {
            Id = roleId,
            Name = "Admin",
            NormalizedName = "ADMIN"
        });

        await context.SaveChangesAsync();

        await context.UserRoles.AddAsync(new IdentityUserRole<string>
        {
            UserId = userId,
            RoleId = roleId
        });

        var userAutor = await context.Users.FindAsync(userId);

        await context.Autores.AddAsync(new Autor
        {
            Id = userAutor.Id,
            Nome = userAutor.NormalizedUserName,
            Email = userAutor.NormalizedEmail
        });
        await context.SaveChangesAsync();

        if (context.Posts.Any())
            return;

        var novoPost = new Post
        {
            AutorId = userAutor.Id,
            Titulo = "Titulo teste",
            Descricao = "Descricao teste",
            DataPublicacao = DateTime.Now

        };
        await context.Posts.AddAsync(novoPost);
        await context.SaveChangesAsync();

        await context.Comentarios.AddAsync(new Comentario
        {
            AutorId = userAutor.Id,
            PostId = novoPost.Id,
            Descricao = "Descricao teste",
            DataPublicacao = DateTime.Now
        });
        await context.SaveChangesAsync();

        await context.Comentarios.AddAsync(new Comentario
        {
            AutorId = userAutor.Id,
            PostId = novoPost.Id,
            Descricao = "Descricao teste 2",
            DataPublicacao = DateTime.Now
        });
        await context.SaveChangesAsync();


    }

}


