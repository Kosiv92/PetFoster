using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PetFoster.Core.DTO.Specie;

namespace PetFoster.Infrastructure.DbContexts
{
    public class ReadDbContext : DbContext
    {
        public DbSet<SpecieDto> Species { get; set; }

        public DbSet<BreedDto> Breeds { get; set; }

        public ReadDbContext(DbContextOptions<ReadDbContext> options,
            IConfiguration configuration) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder
                    .ApplyConfigurationsFromAssembly(typeof(ReadDbContext).Assembly,
                        type => type.FullName?.Contains("Configurations.Read") ?? false);
        }
    }
}
