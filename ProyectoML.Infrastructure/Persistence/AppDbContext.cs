using Microsoft.EntityFrameworkCore;
using ProyectoML.Domain.Entities;

namespace ProyectoML.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<MovimientoStock> MovimientoStock{ get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
       : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<Categoria>();
        //modelBuilder.Entity<Producto>();
        //modelBuilder.Entity<MovimientoStock>();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

    }

}
