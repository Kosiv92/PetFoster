using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PetFoster.Domain.Entities;

namespace PetFoster.Infrastructure
{
    public class ApplicationDbContext(IConfiguration configuration) : DbContext
    {
        private const string DATABASE_NAME = "Database";

        public DbSet<Volunteer> Volunteers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(configuration.GetConnectionString(DATABASE_NAME));
        }
    }
}
