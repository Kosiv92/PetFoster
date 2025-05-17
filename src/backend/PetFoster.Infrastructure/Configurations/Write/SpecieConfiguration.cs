using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFoster.Core.ValueObjects;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Ids;

namespace PetFoster.Infrastructure.Configurations.Write
{
    internal class SpecieConfiguration : IEntityTypeConfiguration<Specie>
    {
        public void Configure(EntityTypeBuilder<Specie> builder)
        {
            _ = builder.ToTable("species");

            _ = builder.HasKey(m => m.Id);

            _ = builder.Property(m => m.Id)
                .HasConversion(
                    id => id.Value,
                    value => SpecieId.Create(value));

            _ = builder.Property(m => m.Name)
                .HasMaxLength(SpecieName.MAX_NAME_LENGTH)
                .HasConversion(
                name => name.Value,
                value => SpecieName.Create(value).Value);

            _ = builder.HasMany(m => m.Breeds)
                .WithOne(m => m.Specie)
                .HasForeignKey("specie_id")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}
