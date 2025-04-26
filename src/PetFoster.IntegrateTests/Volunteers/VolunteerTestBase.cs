using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
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

        public Task InitializeAsync() => Task.CompletedTask;

        public async Task DisposeAsync()
        {
            ServiceScope.Dispose();
            await Factory.ResetDataBaseAsync();
        }        
    }
}
