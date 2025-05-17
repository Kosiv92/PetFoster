using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFoster.Core.DTO.Specie;

namespace PetFoster.Infrastructure.Configurations.Read
{
    public class BreedDtoConfiguration : IEntityTypeConfiguration<BreedDto>
    {
        public void Configure(EntityTypeBuilder<BreedDto> builder)
        {
            _ = builder.ToTable("breeds");

            _ = builder.HasKey(m => m.Id);

            _ = builder.Property(m => m.Id)
                .HasColumnName("id");

            _ = builder.Property(m => m.Name)
                    .HasColumnName("name");

            //builder.HasOne<SpecieDto>()
            //    .WithMany();
        }
    }
}
