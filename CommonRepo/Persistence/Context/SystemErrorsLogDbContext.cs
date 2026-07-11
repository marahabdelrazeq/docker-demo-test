using CommonRepo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommonRepo.Persistence.Context;

public class SystemErrorsLogDbContext : DbContext
{
    public SystemErrorsLogDbContext(DbContextOptions<SystemErrorsLogDbContext> options)
        : base(options)
    {
    }

    public DbSet<SystemErrorsLog> SystemErrorsLog { get; set; }
}
