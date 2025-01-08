using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PetFoster.Domain.Entities;

namespace PetFoster.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {  
        public DbSet<Volunteer> Volunteers { get; set; }

        public DbSet<Specie> Species { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, 
            IConfiguration configuration) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);           
    }
}
