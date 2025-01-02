using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFoster.Domain;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Ids;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Infrastructure.Configurations
{
    public class PetConfiguration : IEntityTypeConfiguration<Pet>
    {
        public void Configure(EntityTypeBuilder<Pet> builder)
        {
            builder.ToTable("pets");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .HasConversion(
                    id => id.Value,
                    value => PetId.Create(value));

            builder.HasOne(m => m.Volunteer)
               .WithMany(v => v.FosteredAnimals)
               .OnDelete(DeleteBehavior.NoAction);     
            
            builder.Property(m => m.Name)
                .HasMaxLength(PetName.MIN_NAME_LENGTH)
                .HasConversion(
                name => name.Value,
                value => PetName.Create(value).Value);

            builder.HasOne(m => m.Specie)
                .WithOne(s => s.Pet)
                .HasForeignKey("pet_id")
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(true);

            builder.HasOne(m => m.Breed)
                .WithOne(s => s.Pet)
                .HasForeignKey("pet_id")
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(true);

            builder.Property(m => m.Description)
                .HasMaxLength(Description.MAX_DESCRIPTION_LENGTH)
                .HasConversion(
                description => description.Value,
                value => Description.Create(value).Value);

            builder.Property(m => m.Coloration)
                .HasMaxLength(PetColoration.MAX_NAME_LENGTH)
                .HasConversion(
                coloration => coloration.Value,
                value => PetColoration.Create(value).Value);


        }
    }
}
