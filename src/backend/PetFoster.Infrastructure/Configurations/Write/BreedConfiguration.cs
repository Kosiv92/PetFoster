using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFoster.Core.ValueObjects;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Ids;

namespace PetFoster.Infrastructure.Configurations.Write
{
    public class BreedConfiguration : IEntityTypeConfiguration<Breed>
    {
        public void Configure(EntityTypeBuilder<Breed> builder)
        {
            _ = builder.ToTable("breeds");

            _ = builder.HasKey(m => m.Id);

            _ = builder.Property(m => m.Id)
                .HasConversion(
                    id => id.Value,
                    value => BreedId.Create(value));

            _ = builder.Property(m => m.Name)
                .HasMaxLength(BreedName.MAX_NAME_LENGTH)
                .HasConversion(
                name => name.Value,
                value => BreedName.Create(value).Value);

            _ = builder.HasOne(m => m.Specie)
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
}
