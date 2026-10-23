using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProyectoML.Domain.Entities;

namespace ProyectoML.Infrastructure.Persistence.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.NombreUsuario)
            .IsRequired()
            .HasMaxLength(50);


        builder.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(256);
        builder.HasIndex(p => p.Email).IsUnique();


        builder.Property(p => p.PasswordHash)
            .IsRequired()
            .HasMaxLength(200);


    }
}
