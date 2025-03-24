using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Ids;
using PetFoster.Domain.ValueObjects;
using PetFoster.Infrastructure.Extensions;

namespace PetFoster.Infrastructure.Configurations.Write
{
    public class VolunteerConfiguration : IEntityTypeConfiguration<Volunteer>
    {
        public void Configure(EntityTypeBuilder<Volunteer> builder)
        {
            builder.ToTable("volunteers", t =>
            {
                t.HasCheckConstraint("CK_Volunteer_WorkExperience_NonNegative", $"{WorkExperience.COLUMN_NAME} >= 0");
                t.HasCheckConstraint("CK_Volunteer_Email_ValidFormat", $"{Email.COLUMN_NAME} ~* '{Email.EMAIL_PATTERN}'");
            });

            builder.HasQueryFilter(m => !m.IsDeleted);

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .HasConversion(
                    id => id.Value,
                    value => VolunteerId.Create(value));

            builder.ComplexProperty(m => m.FullName, pb =>
            {
                pb.Property(p => p.FirstName)
                .IsRequired(true)
                .HasMaxLength(FullName.MAX_NAME_LENGTH)
                .HasColumnName("first_name");

                pb.Property(p => p.LastName)
                .IsRequired(true)
                .HasMaxLength(FullName.MAX_NAME_LENGTH)
                .HasColumnName("last_name");

                pb.Property(p => p.Patronymic)
                .IsRequired(false)
                .HasMaxLength(FullName.MAX_NAME_LENGTH)
                .HasColumnName("patronymic");
            });

            builder.Property(m => m.Email)
                .HasColumnName(Email.COLUMN_NAME)
                .HasMaxLength(Email.MAX_EMAIL_LENGTH)
                .HasConversion(
                email => email.Value,
                value => Email.Create(value).Value);

            builder.Property(m => m.Description)
                .HasMaxLength(Description.MAX_DESCRIPTION_LENGTH)
                .HasConversion(
                description => description.Value,
                value => Description.Create(value).Value);

            builder.Property(m => m.WorkExperienceInYears)
                .HasColumnName(WorkExperience.COLUMN_NAME)
                .HasConversion(
                description => description.Value,
                value => WorkExperience.Create(value).Value);

            builder.Property(m => m.PhoneNumber)
                .HasColumnName(PhoneNumber.COLUMN_NAME)
                .HasConversion(
                phoneNumber => phoneNumber.Value,
                value => PhoneNumber.Create(value).Value);

            builder.Property(m => m.AssistanceRequisitesList)
                .HasField("_assistanceRequisites")
                .IsRequired()
                .JsonValueObjectCollectionConversion();

            builder.Property(m => m.SocialNetContacts)
                .HasField("_socialNetContacts")
                .IsRequired()
                .JsonValueObjectCollectionConversion();

            builder.HasMany(m => m.FosteredAnimals)
                .WithOne(m => m.Volunteer)
                .HasForeignKey("volunteer_id")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.Navigation(v => v.FosteredAnimals)
                .HasField("_fosteredPets")
                .AutoInclude();

            builder.Property(m => m.IsDeleted);
        }
    }
}
