using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProyectoML.Domain.Entities;

namespace ProyectoML.Infrastructure.Persistence.Configurations;

public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
{
    public void Configure(EntityTypeBuilder<Producto> builder)
    {
        builder.ToTable("Productos");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.SKU).IsRequired().HasMaxLength(50);
        builder.HasIndex(p => p.SKU).IsUnique();

        builder.Property(p => p.Nombre).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Descripcion).HasMaxLength(2000);
        builder.Property(p => p.Precio).HasPrecision(18, 2);

        builder.HasOne(p => p.Categoria)
               .WithMany()
               .HasForeignKey(p => p.CategoriaId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Movimientos)
               .WithOne(m => m.Producto)
               .HasForeignKey(m => m.ProductoId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(p => p.Movimientos)
               .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
