using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProyectoML.Domain.Entities;

namespace ProyectoML.Infrastructure.Persistence.Configurations;

public class MovimientoStockConfiguration : IEntityTypeConfiguration<MovimientoStock>
{
    public void Configure(EntityTypeBuilder<MovimientoStock> builder)
    {

        builder.ToTable("MovimientoStock");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Referencia).HasMaxLength(100);
        builder.Property(c => c.Motivo).HasConversion<string>().HasMaxLength(20);
        
    }
}
