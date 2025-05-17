using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFoster.Core.DTO.Volunteer;
using PetFoster.Core.ValueObjects;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Enums;
using PetFoster.Domain.Ids;
using PetFoster.Infrastructure.Extensions;
using System.Text.Json;

namespace PetFoster.Volunteers.Infrastructure.Configurations.Write
{
    public class PetConfiguration : IEntityTypeConfiguration<Pet>
    {
        public void Configure(EntityTypeBuilder<Pet> builder)
        {
            _ = builder.ToTable("pets", t =>
            {
                _ = t.HasCheckConstraint("CK_Pet_Weight_NonNegative", $"{Characteristics.WEIGHT_COLUMN_NAME} >= 0");
                _ = t.HasCheckConstraint("CK_Pet_Height_NonNegative", $"{Characteristics.HEIGHT_COLUMN_NAME} >= 0");
                _ = t.HasCheckConstraint("CK_Volunteer_PhoneNumber_NumericOnly", $"{PhoneNumber.COLUMN_NAME} ~ '^[0-9]{{{PhoneNumber.PHONE_NUMBER_LENGTH}}}$'");
            });

            _ = builder.HasQueryFilter(m => !m.IsDeleted);

            _ = builder.HasKey(m => m.Id);

            _ = builder.Property(m => m.Id)
                .HasConversion(
                    id => id.Value,
                    value => PetId.Create(value));

            _ = builder.HasOne(m => m.Volunteer)
               .WithMany(v => v.FosteredAnimals)
               .OnDelete(DeleteBehavior.NoAction);

            _ = builder.Property(m => m.Name)
                .HasMaxLength(PetName.MAX_NAME_LENGTH)
                .HasConversion(
                name => name.Value,
                value => PetName.Create(value).Value);

            _ = builder.Property(m => m.SpecieId)
                .HasConversion(
                id => id.Value,
                value => SpecieId.Create(value))
                .HasColumnName("specie_id");

            _ = builder.HasOne(m => m.Specie)
            .WithMany()
            .HasForeignKey(p => p.SpecieId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

            _ = builder.Property(m => m.BreedId)
                .HasConversion(
                id => id.Value,
                value => BreedId.Create(value))
                .HasColumnName("breed_id");

            _ = builder.HasOne(m => m.Breed)
                .WithMany()
                .HasForeignKey(p => p.BreedId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            _ = builder.Property(m => m.Description)
                .HasMaxLength(Description.MAX_DESCRIPTION_LENGTH)
                .HasConversion(
                description => description.Value,
                value => Description.Create(value).Value);

            _ = builder.Property(m => m.Coloration)
                .HasMaxLength(PetColoration.MAX_NAME_LENGTH)
                .HasConversion(
                coloration => coloration.Value,
                value => PetColoration.Create(value).Value);

            _ = builder.Property(m => m.Health)
                .HasMaxLength(PetHealth.MAX_HEALTH_LENGTH)
                .HasConversion(
                health => health.Value,
                value => PetHealth.Create(value).Value);

            _ = builder.ComplexProperty(m => m.Address, pb =>
            {
                _ = pb.Property(pm => pm.Region)
                .HasMaxLength(Address.MAX_REGION_LENGTH)
                .IsRequired(true)
                .HasColumnName("region");

                _ = pb.Property(pm => pm.City)
                .HasMaxLength(Address.MAX_CITY_LENGTH)
                .IsRequired(true)
                .HasColumnName("city");

                _ = pb.Property(pm => pm.Street)
                .HasMaxLength(Address.MAX_STREET_LENGTH)
                .IsRequired(true)
                .HasColumnName("street");

                _ = pb.Property(pm => pm.HouseNumber)
                .HasMaxLength(Address.MAX_HOUSE_LENGTH)
                .IsRequired(true)
                .HasColumnName("house_number");

                _ = pb.Property(pm => pm.HouseNumber)
                .HasMaxLength(Address.MAX_APARTMENT_LENGTH)
                .IsRequired(false)
                .HasColumnName("apartment");
            });

            _ = builder.ComplexProperty(m => m.Characteristics, pb =>
            {
                _ = pb.Property(pm => pm.Weight)
                .IsRequired(true)
                .HasColumnName(Characteristics.WEIGHT_COLUMN_NAME);

                _ = pb.Property(pm => pm.Height)
                .IsRequired(true)
                .HasColumnName(Characteristics.HEIGHT_COLUMN_NAME);
            });

            _ = builder.Property(m => m.OwnerPhoneNumber)
                .HasColumnName(PhoneNumber.COLUMN_NAME)
                .HasConversion(
                phoneNumber => phoneNumber.Value,
                value => PhoneNumber.Create(value).Value);

            _ = builder.Property(m => m.IsCastrated)
                .HasColumnName("сastrated")
                .IsRequired(true);

            _ = builder.Property(m => m.IsVaccinated)
                .HasColumnName("vaccinated")
                .IsRequired(true);

            _ = builder.Property(m => m.AssistanceRequisitesList)
                .JsonValueObjectCollectionConversion();

            _ = builder.Property(m => m.FileList)
                .HasField("_fileList")
                .HasConversion(files => JsonSerializer.Serialize(files
                .Select(f => new PetFileDto(f.PathToStorage.Path)), JsonSerializerOptions.Default),
                json => JsonSerializer.Deserialize<List<PetFileDto>>(json, JsonSerializerOptions.Default)!
                .Select(dto => new PetFile(FilePath.Create(dto.FilePath).Value)).ToList(),
                new ValueComparer<IReadOnlyList<PetFile>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()))
                .IsRequired(false);

            _ = builder.Property(m => m.AssistanceStatus)
                .HasConversion(
                status => status.ToString(),
                value => (AssistanceStatus)Enum.Parse(typeof(AssistanceStatus), value));

            _ = builder.Property(m => m.BirthDay)
                .HasConversion(
                    d => d.HasValue ? d.Value.UtcDateTime : (DateTime?)null,
                    d => d.HasValue ? new DateTimeOffset(d.Value, TimeSpan.Zero) : null
                )
                .IsRequired(false);

            _ = builder.Property(m => m.CreatedDate)
                .HasConversion(
                    d => d.UtcDateTime,
                    d => new DateTimeOffset(d, TimeSpan.Zero)
                    )
                .IsRequired(true);

            _ = builder.Property(m => m.IsDeleted);

            _ = builder.Property(m => m.Position)
                .HasConversion(
                    position => position.Value,
                    value => Position.Create(value).Value)
                .IsRequired(true);
        }
    }
}
