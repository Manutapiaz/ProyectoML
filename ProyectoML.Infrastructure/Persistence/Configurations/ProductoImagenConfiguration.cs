using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProyectoML.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoML.Infrastructure.Persistence.Configurations;

public class ProductoImagenConfiguration : IEntityTypeConfiguration<ProductoImagen> 
{
    public void Configure(EntityTypeBuilder<ProductoImagen> builder)
    {
        builder.ToTable("ProductoImagenes");     
        builder.HasKey(pi => pi.Id);
        builder.Property(pi => pi.Url).IsRequired().HasMaxLength(500);
        builder.Property(pi => pi.ProductoId).IsRequired();
      
    }
}
