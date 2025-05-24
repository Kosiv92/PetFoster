using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PetFoster.Species.Domain.Entities;

namespace PetFoster.Species.Infrastructure.DbContexts
{
    public class WriteDbContext : DbContext
    {
        public DbSet<Specie> Species { get; set; }

        public WriteDbContext(DbContextOptions<WriteDbContext> options,
            IConfiguration configuration) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
             modelBuilder.ApplyConfigurationsFromAssembly(typeof(WriteDbContext).Assembly,
                        type => type.FullName?.Contains("Configurations.Write") ?? false);

             modelBuilder.HasDefaultSchema("species");
        }
    }
}
