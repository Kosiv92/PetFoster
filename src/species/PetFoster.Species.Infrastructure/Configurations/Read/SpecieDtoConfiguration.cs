using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFoster.Core.DTO.Specie;

namespace PetFoster.Species.Infrastructure.Configurations.Read;

public class SpecieDtoConfiguration : IEntityTypeConfiguration<SpecieDto>
{
    public void Configure(EntityTypeBuilder<SpecieDto> builder)
    {
         builder.ToTable("species");

         builder.HasKey(m => m.Id);

         builder.Property(m => m.Id)
            .HasColumnName("id");

         builder.Property(m => m.Name)
           .HasColumnName("name");

         builder.HasMany(m => m.Breeds)
            .WithOne()
            .HasForeignKey(b => b.SpecieId);
    }
}
