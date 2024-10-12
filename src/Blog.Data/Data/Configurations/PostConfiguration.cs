using System;
using Blog.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data.Data.Configurations;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Post> builder)
    {
        builder.ToTable("Posts");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Descricao)
            .IsRequired()
            .HasMaxLength(500);
        builder.Property(p => p.Titulo)
            .IsRequired();
        builder.Property(p => p.DataPublicacao)
            .IsRequired();
        
        builder
            .HasOne(a => a.Autor)
            .WithMany()
            .HasForeignKey(p => p.AutorId)
            .OnDelete(DeleteBehavior.Restrict); // Evita exclusão em cascata;

        builder
            .HasMany(c => c.Comentarios)
            .WithOne(p => p.Post)
            .HasForeignKey(p => p.PostId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}