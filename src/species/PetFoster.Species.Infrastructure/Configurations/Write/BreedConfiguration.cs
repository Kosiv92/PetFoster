using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFoster.SharedKernel.ValueObjects;
using PetFoster.SharedKernel.ValueObjects.Ids;
using PetFoster.Species.Domain.Entities;

namespace PetFoster.Species.Infrastructure.Configurations.Write;

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
            .HasMaxLength(BreedName.MAX_NAME_LENGTH)
            .HasConversion(
            name => name.Value,
            value => BreedName.Create(value).Value);

         builder.HasOne(m => m.Specie)
            .WithMany(m => m.Breeds)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();

        //builder.HasMany(m => m.Pets)
        //    .WithOne()       
        //    .HasForeignKey(p =>)
        //    .OnDelete(DeleteBehavior.NoAction)
        //    .IsRequired();
    }
}
