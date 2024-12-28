using Microsoft.EntityFrameworkCore;
using PetFoster.Domain.Entities;

namespace PetFoster.Infrastructure
{
    public class ApplicationDbContext: DbContext
    {
        public DbSet<Volunteer> Volunteers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql();
        }
    }
}
