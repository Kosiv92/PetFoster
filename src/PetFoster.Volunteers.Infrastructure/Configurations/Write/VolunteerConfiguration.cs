using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFoster.Core.ValueObjects;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Ids;
using PetFoster.Infrastructure.Extensions;

namespace PetFoster.Volunteers.Infrastructure.Configurations.Write
{
    public class VolunteerConfiguration : IEntityTypeConfiguration<Volunteer>
    {
        public void Configure(EntityTypeBuilder<Volunteer> builder)
        {
            _ = builder.ToTable("volunteers", t =>
            {
                _ = t.HasCheckConstraint("CK_Volunteer_WorkExperience_NonNegative", $"{WorkExperience.COLUMN_NAME} >= 0");
                _ = t.HasCheckConstraint("CK_Volunteer_Email_ValidFormat", $"{Email.COLUMN_NAME} ~* '{Email.EMAIL_PATTERN}'");
            });

            _ = builder.HasQueryFilter(m => !m.IsDeleted);

            _ = builder.HasKey(m => m.Id);

            _ = builder.Property(m => m.Id)
                .HasConversion(
                    id => id.Value,
                    value => VolunteerId.Create(value));

            _ = builder.ComplexProperty(m => m.FullName, pb =>
            {
                _ = pb.Property(p => p.FirstName)
                .IsRequired(true)
                .HasMaxLength(FullName.MAX_NAME_LENGTH)
                .HasColumnName("first_name");

                _ = pb.Property(p => p.LastName)
                .IsRequired(true)
                .HasMaxLength(FullName.MAX_NAME_LENGTH)
                .HasColumnName("last_name");

                _ = pb.Property(p => p.Patronymic)
                .IsRequired(false)
                .HasMaxLength(FullName.MAX_NAME_LENGTH)
                .HasColumnName("patronymic");
            });

            _ = builder.Property(m => m.Email)
                .HasColumnName(Email.COLUMN_NAME)
                .HasMaxLength(Email.MAX_EMAIL_LENGTH)
                .HasConversion(
                email => email.Value,
                value => Email.Create(value).Value);

            _ = builder.Property(m => m.Description)
                .HasMaxLength(Description.MAX_DESCRIPTION_LENGTH)
                .HasConversion(
                description => description.Value,
                value => Description.Create(value).Value);

            _ = builder.Property(m => m.WorkExperienceInYears)
                .HasColumnName(WorkExperience.COLUMN_NAME)
                .HasConversion(
                description => description.Value,
                value => WorkExperience.Create(value).Value);

            _ = builder.Property(m => m.PhoneNumber)
                .HasColumnName(PhoneNumber.COLUMN_NAME)
                .HasConversion(
                phoneNumber => phoneNumber.Value,
                value => PhoneNumber.Create(value).Value);

            _ = builder.Property(m => m.AssistanceRequisitesList)
                .HasField("_assistanceRequisites")
                .IsRequired()
                .JsonValueObjectCollectionConversion();

            _ = builder.Property(m => m.SocialNetContacts)
                .HasField("_socialNetContacts")
                .IsRequired()
                .JsonValueObjectCollectionConversion();

            _ = builder.HasMany(m => m.FosteredAnimals)
                .WithOne(m => m.Volunteer)
                .HasForeignKey("volunteer_id")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            _ = builder.Navigation(v => v.FosteredAnimals)
                .HasField("_fosteredPets")
                .AutoInclude();

            _ = builder.Property(m => m.IsDeleted);
        }
    }
}
