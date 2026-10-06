

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using ProyectoML.Domain.Entities;

namespace ProyectoML.Infrastructure.Persistence.Configurations;

public class CredencialMercadoLibreConfiguration : IEntityTypeConfiguration<CredencialMercadoLibre>
{
    public void Configure(EntityTypeBuilder<CredencialMercadoLibre> builder)
    {
        builder.ToTable("CredencialesMercadoLibre");
        builder.HasKey(c => c.UserId);
        builder.Property(c => c.UserId).ValueGeneratedNever();
        builder.Property(c => c.AccessToken).IsRequired();
        builder.Property(c => c.ExpiresAt).IsRequired();
        builder.Property(c => c.RefreshToken).IsRequired();

    }
}
