using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFoster.Domain;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Enums;
using PetFoster.Domain.Ids;
using PetFoster.Domain.ValueObjects;
using PetFoster.Infrastructure.Extensions;

namespace PetFoster.Infrastructure.Configurations
{
    public class PetConfiguration : IEntityTypeConfiguration<Pet>
    {
        public void Configure(EntityTypeBuilder<Pet> builder)
        {
            builder.ToTable("pets", t =>
            {
                t.HasCheckConstraint("CK_Pet_Weight_NonNegative", $"{Characteristics.WEIGHT_COLUMN_NAME} >= 0");
                t.HasCheckConstraint("CK_Pet_Height_NonNegative", $"{Characteristics.HEIGHT_COLUMN_NAME} >= 0");
                t.HasCheckConstraint("CK_Volunteer_PhoneNumber_NumericOnly", $"{PhoneNumber.COLUMN_NAME} ~ '^[0-9]{PhoneNumber.PHONE_NUMBER_LENGTH}$'");
            });            

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
                .WithMany(b => b.Pets)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(true);

            builder.HasOne(m => m.Breed)
                .WithMany(b => b.Pets)                
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

            builder.Property(m => m.Health)
                .HasMaxLength(PetHealth.MAX_HEALTH_LENGTH)
                .HasConversion(
                health => health.Value,
                value => PetHealth.Create(value).Value);

            builder.ComplexProperty(m => m.Address, pb =>
            {
                pb.Property(pm => pm.Region)
                .HasMaxLength(Address.MAX_REGION_LENGTH)
                .IsRequired(true)                
                .HasColumnName("region");

                pb.Property(pm => pm.City)
                .HasMaxLength(Address.MAX_CITY_LENGTH)
                .IsRequired(true)
                .HasColumnName("city");

                pb.Property(pm => pm.Street)
                .HasMaxLength(Address.MAX_STREET_LENGTH)
                .IsRequired(true)
                .HasColumnName("street");

                pb.Property(pm => pm.HouseNumber)
                .HasMaxLength(Address.MAX_HOUSE_LENGTH)
                .IsRequired(true)
                .HasColumnName("house_number");

                pb.Property(pm => pm.HouseNumber)
                .HasMaxLength(Address.MAX_APARTMENT_LENGTH)
                .IsRequired(false)
                .HasColumnName("apartment");
            });

            builder.ComplexProperty(m => m.Characteristics, pb =>
            {
                pb.Property(pm => pm.Weight)                
                .IsRequired(true)
                .HasColumnName(Characteristics.WEIGHT_COLUMN_NAME);

                pb.Property(pm => pm.Height)                
                .IsRequired(true)
                .HasColumnName(Characteristics.HEIGHT_COLUMN_NAME);                
            });

            builder.Property(m => m.OwnerPhoneNumber)
                .HasColumnName(PhoneNumber.COLUMN_NAME)
                .HasConversion(
                phoneNumber => phoneNumber.Value,
                value => PhoneNumber.Create(value).Value);

            builder.Property(m => m.IsCastrated)
                .HasColumnName("сastrated")
                .IsRequired(true);

            builder.Property(m => m.IsVaccinated)
                .HasColumnName("vaccinated")
                .IsRequired(true);

            builder.Property(m => m.AssistanceRequisitesList)
                .JsonValueObjectCollectionConversion();
            
            builder.Property(m => m.AssistanceStatus)                
                .HasConversion(
                status => status.ToString(),
                value => (AssistanceStatus)Enum.Parse(typeof(AssistanceStatus), value));

            builder.Property(m => m.BirthDay)
                .IsRequired(false);

            builder.Property(m => m.CreatedDate)
                .IsRequired(true);

        }
    }
}
