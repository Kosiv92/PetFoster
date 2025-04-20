using AutoFixture;
using Bogus;
using PetFoster.Application.Volunteers.CreateVolunteer;
using PetFoster.Domain.Ids;

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
    }
}
