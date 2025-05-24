using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Core.Abstractions;
using PetFoster.SharedKernel.ValueObjects.Ids;
using PetFoster.Volunteers.Domain.Entities;

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
            if (volunteerIds?.Any() == true)
            {
                foreach (Guid id in volunteerIds)
                {
                    Volunteer newVolunteer = Fixture.CreateVolunteer(id);
                    await Repository.AddAsync(newVolunteer, cancellationToken);
                }
            }
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            ServiceScope.Dispose();
            await Factory.ResetDataBaseAsync();
        }
    }
}
