using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFoster.Application.DTO.Specie;

namespace PetFoster.Infrastructure.Configurations.Read
{
    public class BreedDtoConfiguration : IEntityTypeConfiguration<BreedDto>
    {
        public void Configure(EntityTypeBuilder<BreedDto> builder)
        {
            builder.ToTable("breeds");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .HasColumnName("id");

            builder.Property(m => m.Name)
                    .HasColumnName("name");

            //builder.HasOne<SpecieDto>()
            //    .WithMany();
        }
    }
}
