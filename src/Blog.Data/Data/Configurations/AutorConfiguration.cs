using System;
using Blog.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data.Data.Configurations;

public class AutorConfiguration : IEntityTypeConfiguration<Autor>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Autor> builder)
    {
        builder.ToTable("Autores");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Email)
            .IsRequired();
        builder.Property(a => a.Nome)
.           IsRequired();
        builder.
            HasOne(a => a.User)
            .WithOne()
            .HasForeignKey<Autor>(a => a.Id)
            .IsRequired();
    }

}
