using AutoFixture;
using Bogus;
using PetFoster.Application.Volunteers.CreateVolunteer;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Ids;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.IntegrateTests
{
    public static class FixtureExtensions
    {
        public static CreateVolunteerCommand CreateCreateVolunteerCommand(
            this IFixture fixture, Guid volunteerId)
        {
            var faker = new Faker();

            return fixture.Build<CreateVolunteerCommand>()
                .With(c => c.id, VolunteerId.Create(volunteerId))     
                .With(c => c.Email, () => faker.Internet.Email())
                .With(c => c.PhoneNumber, () => faker.Phone.PhoneNumber("###########"))
                .Create();
        }

        public static Volunteer CreateVolunteer(
            this IFixture fixture, Guid volunteerId)
        {
            var faker = new Faker();

            List<AssistanceRequisites> assistanceRequisites = new List<AssistanceRequisites>();
            List<SocialNetContact> socialNetContacts = new List<SocialNetContact>();

            for(int i = 0; i < 2; i++)
            {
                var assistanceRequisite = AssistanceRequisites.Create(
                    fixture.Create<string>(),
                    Description.Create(fixture.Create<string>()).Value).Value;

                assistanceRequisites.Add(assistanceRequisite);

                var socialNetContact = SocialNetContact.Create(
                    fixture.Create<string>(), 
                    fixture.Create<string>()).Value;

                socialNetContacts.Add(socialNetContact);
            }

            var id = VolunteerId.Create(volunteerId);
            var fullName = FullName.Create(faker.Name.FirstName(), faker.Name.LastName(), faker.Name.FirstName()).Value;
            var email = Email.Create(faker.Internet.Email()).Value;
            var description = Description.Create(fixture.Create<string>()).Value;
            var workExpirience = WorkExperience.Create(fixture.Create<int>()).Value;
            var phoneNumber = PhoneNumber.Create(faker.Phone.PhoneNumber("###########")).Value;

            return new Volunteer(id, fullName, email, 
                description, workExpirience, phoneNumber,
                assistanceRequisites, socialNetContacts);
        }
    }
}
