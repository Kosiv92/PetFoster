using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PetFoster.Application.DTO.Specie;

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
            => modelBuilder
            .ApplyConfigurationsFromAssembly(typeof(ReadDbContext).Assembly, 
                type => type.FullName?.Contains("Configurations.Read") ?? false);
    }
}
