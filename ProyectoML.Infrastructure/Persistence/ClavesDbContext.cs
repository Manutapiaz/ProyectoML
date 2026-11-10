using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ProyectoML.Infrastructure.Persistence;

public class ClavesDbContext : DbContext, IDataProtectionKeyContext
{
    public ClavesDbContext(DbContextOptions<ClavesDbContext> options)
        : base(options)
    {
    }
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
}
