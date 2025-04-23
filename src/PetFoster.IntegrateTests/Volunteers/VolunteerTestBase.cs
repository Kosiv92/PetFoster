using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.DTO.Volunteer;
using PetFoster.Application.Volunteers.CreateVolunteer;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Ids;
using PetFoster.Domain.Interfaces;

namespace PetFoster.IntegrateTests.Volunteers
{
    public class VolunteerTestBase 
        : IClassFixture<IntegrationTestsWebFactory>, IAsyncLifetime
    {
        protected readonly IntegrationTestsWebFactory Factory;
        protected readonly Fixture Fixture;
        protected readonly IServiceScope ServiceScope;
        protected readonly IRepository<Volunteer, VolunteerId> Repository;

        public VolunteerTestBase(IntegrationTestsWebFactory factory)
        {
            Factory = factory;
            Fixture = new Fixture();
            ServiceScope = Factory.Services.CreateScope();
            Repository = ServiceScope.ServiceProvider
                .GetRequiredService<IRepository<Volunteer, VolunteerId>>();
        }

        protected async Task SeedDatabase(IEnumerable<Guid> volunteerIds, 
            CancellationToken cancellationToken = default)
        {
            if(volunteerIds?.Any() == true)
            {
                foreach(var id in volunteerIds)
                {
                    var newVolunteer = Fixture.CreateVolunteer(id);
                    await Repository.AddAsync(newVolunteer, cancellationToken);
                }
            }
        }

        protected CreateVolunteerCommand CreateCommand(VolunteerId id)
        {
            var fullName = new FullNameDto("John", "Smith", "Isaac");
            var email = "anyemail@mail.com";
            var phoneNumber = "88009994488";
            var description = "Any description";
            var workExpirience = 5;
            var assistanceRequisitesList = new List<AssistanceRequisitesDto>()
            {
                new AssistanceRequisitesDto("Requisite 1", "5480"),
                new AssistanceRequisitesDto("Requisite 2", "7615"),
            };
            var socialNetContactsList = new List<SocialNetContactsDto>()
            {
                new SocialNetContactsDto("Facebook", "JohnSmith.91"),
                new SocialNetContactsDto("Instagram", "johnysmith.91"),
            };

            return new CreateVolunteerCommand(id, fullName, email,
                phoneNumber, description, workExpirience,
                assistanceRequisitesList, socialNetContactsList);
        }

        public Task InitializeAsync() => Task.CompletedTask;

        public async Task DisposeAsync()
        {
            ServiceScope.Dispose();
            await Factory.ResetDataBaseAsync();
        }        
    }
}
