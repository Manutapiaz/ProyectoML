using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProyectoML.Domain.Entities;

namespace ProyectoML.Infrastructure.Persistence.Configurations;

public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
{

    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("Categorias");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
        builder.HasIndex(c => new { c.UsuarioId, c.Nombre }).IsUnique();
        builder.Property(c => c.IdCategoriaMLC).HasMaxLength(30);

        builder.HasOne(p => p.Usuario)
         .WithMany()
         .HasForeignKey(p => p.UsuarioId)
         .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(p => p.Usuario)
         .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
