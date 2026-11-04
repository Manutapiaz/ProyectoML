using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using ProyectoML.Domain.Entities;

namespace ProyectoML.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<MovimientoStock> MovimientoStock { get; set; }
    public DbSet<CredencialMercadoLibre> CredencialesMercadoLibre { get; set; }
    public DbSet<PublicacionMercadoLibre> PublicacionesMercadoLibre { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }


    private readonly IDataProtector _dataProtectionProvider;
    public AppDbContext(DbContextOptions<AppDbContext> options, IDataProtectionProvider dataProtectionProvider)
       : base(options)
    {
        _dataProtectionProvider = dataProtectionProvider.CreateProtector("MercadoLibre.Tokens");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        modelBuilder.Entity<CredencialMercadoLibre>(builder =>
        {
            builder.Property(c => c.AccessToken)
                .HasConversion(v => _dataProtectionProvider.Protect(v), v => _dataProtectionProvider.Unprotect(v));
            builder.Property(c => c.RefreshToken)
                .HasConversion(v => _dataProtectionProvider.Protect(v), v => _dataProtectionProvider.Unprotect(v));
        });
    }

}
