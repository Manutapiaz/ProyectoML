using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProyectoML.Domain.Entities;

namespace ProyectoML.Infrastructure.Persistence.Configurations
{
    public class PublicacionMercadoLibreConfiguration : IEntityTypeConfiguration<PublicacionMercadoLibre>
    {
        public void Configure(EntityTypeBuilder<PublicacionMercadoLibre> builder)
        {
            builder.ToTable("PublicacionesMercadoLibre");
            builder.HasKey(p => p.Id);

            // Relaciones
            builder.HasOne(p => p.Producto)
                .WithMany()
                .HasForeignKey(p => p.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<CredencialMercadoLibre>()
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Propiedades
            builder.Property(p => p.ExternalId)
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(p => p.Estado)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(p => p.Permalink)
                .HasMaxLength(500);

            // Índices
            builder.HasIndex(p => p.ExternalId).IsUnique();
            builder.HasIndex(p => new { p.ProductoId, p.UserId }).IsUnique();
        }
    }
}