using System;
using Blog.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data.Data.Configurations;

public class ComentarioConfiguration : IEntityTypeConfiguration<Comentario>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Comentario> builder)
    {
        builder.ToTable("Comentarios");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Descricao)
            .IsRequired()
            .HasMaxLength(500);
        builder.Property(c => c.DataPublicacao)
            .IsRequired();
        builder
            .HasOne(a => a.Autor)
            .WithMany()
            .HasForeignKey(c => c.AutorId)
            .OnDelete(DeleteBehavior.Restrict); // Evita exclusão em cascata;
    }
}
