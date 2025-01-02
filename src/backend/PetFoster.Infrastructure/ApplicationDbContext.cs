using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PetFoster.Domain.Entities;

namespace PetFoster.Infrastructure
{
    public class ApplicationDbContext(IConfiguration configuration) : DbContext
    {  
        public DbSet<Volunteer> Volunteers { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseNpgsql(configuration.GetConnectionString(DATABASE_NAME));
        //    optionsBuilder.UseSnakeCaseNamingConvention();
        //    optionsBuilder.UseLoggerFactory(CreateLoggerFactory());
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);        

        
    }
}
