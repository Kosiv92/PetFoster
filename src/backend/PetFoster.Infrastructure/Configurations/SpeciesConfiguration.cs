using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Ids;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Infrastructure.Configurations
{
    internal class SpeciesConfiguration : IEntityTypeConfiguration<Species>
    {
        public void Configure(EntityTypeBuilder<Species> builder)
        {
            builder.ToTable("species");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .HasConversion(
                    id => id.Value,
                    value => SpeciesId.Create(value));

            builder.HasOne(m => m.Pet)
               .WithOne(v => v.Specie)
               .OnDelete(DeleteBehavior.NoAction);

            builder.Property(m => m.Name)
                .HasMaxLength(SpeciesName.MIN_NAME_LENGTH)                
                .HasConversion(
                name => name.Value,
                value => SpeciesName.Create(value).Value);

            builder.HasMany(m => m.Breeds)
                .WithOne(m => m.Species)
                .HasForeignKey("species_id")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}
