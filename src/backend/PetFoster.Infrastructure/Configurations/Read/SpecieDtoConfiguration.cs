using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFoster.Core.DTO.Specie;

namespace PetFoster.Infrastructure.Configurations.Read
{
    public class SpecieDtoConfiguration : IEntityTypeConfiguration<SpecieDto>
    {
        public void Configure(EntityTypeBuilder<SpecieDto> builder)
        {
            _ = builder.ToTable("species");

            _ = builder.HasKey(m => m.Id);

            _ = builder.Property(m => m.Id)
                .HasColumnName("id");

            _ = builder.Property(m => m.Name)
               .HasColumnName("name");

            _ = builder.HasMany(m => m.Breeds)
                .WithOne()
                .HasForeignKey(b => b.SpecieId);
        }
    }
}
