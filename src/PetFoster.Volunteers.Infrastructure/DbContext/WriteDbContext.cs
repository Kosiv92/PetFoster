using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PetFoster.Volunteers.Domain.Entities;

namespace PetFoster.Volunteers.Infrastructure.DbContext
{
    public class WriteDbContext : DbContext
    {
        public DbSet<Volunteer> Volunteers { get; set; }

        public WriteDbContext(DbContextOptions<WriteDbContext> options,
            IConfiguration configuration) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.ApplyConfigurationsFromAssembly(typeof(WriteDbContext).Assembly,
                        type => type.FullName?.Contains("Configurations.Write") ?? false);
        }
    }
}
