using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Ids;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Infrastructure.Configurations.Write
{
    public class BreedConfiguration : IEntityTypeConfiguration<Breed>
    {
        public void Configure(EntityTypeBuilder<Breed> builder)
        {
            builder.ToTable("breeds");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)                
                .HasConversion(
                    id => id.Value,
                    value => BreedId.Create(value));

            builder.Property(m => m.Name)
                .HasMaxLength(BreedName.MIN_NAME_LENGTH)
                .HasConversion(
                name => name.Value,
                value => BreedName.Create(value).Value);

            builder.HasOne(m => m.Specie)
                .WithMany(m => m.Breeds)                
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            builder.HasMany(m => m.Pets)
                .WithOne(p => p.Breed)
                .HasForeignKey("breed_id")
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(true);
        }
    }
}
