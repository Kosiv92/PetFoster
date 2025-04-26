using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Ids;
using PetFoster.Domain.Interfaces;

namespace PetFoster.IntegrateTests.Pets
{
    public class PetTestBase
        : IClassFixture<IntegrationTestsWebFactory>, IAsyncLifetime
    {
        protected readonly IntegrationTestsWebFactory Factory;
        protected readonly Fixture Fixture;
        protected readonly IServiceScope ServiceScope;
        protected readonly IRepository<Volunteer, VolunteerId> VolunteerRepository;
        protected readonly IRepository<Specie, SpecieId> SpecieRepository;

        public PetTestBase(IntegrationTestsWebFactory factory)
        {
            Factory = factory;
            Fixture = new Fixture();
            ServiceScope = Factory.Services.CreateScope();
            VolunteerRepository = ServiceScope.ServiceProvider
                .GetRequiredService<IRepository<Volunteer, VolunteerId>>();
            SpecieRepository = ServiceScope.ServiceProvider
                .GetRequiredService<IRepository<Specie, SpecieId>>();
        }

        protected async Task SeedDatabase(Guid volunteerId,
            Guid specieId, Guid breedId, Guid? petId,
            CancellationToken cancellationToken = default)
        {
            var specie = Fixture.CreateSpecie(specieId);
            var breed = Fixture.CreateBreed(breedId);
            specie.AddBreed(breed);
            await SpecieRepository.AddAsync(specie, cancellationToken);

            var newVolunteer = Fixture.CreateVolunteer(volunteerId);

            if(petId != null)
            {
                var pet = Fixture.CreatePet(petId.Value, specieId, breedId);
                newVolunteer.AddPet(pet);
            }

            await VolunteerRepository.AddAsync(newVolunteer, cancellationToken);
        }

        public Task InitializeAsync() => Task.CompletedTask;

        public async Task DisposeAsync()
        {
            ServiceScope.Dispose();
            await Factory.ResetDataBaseAsync();
        }
    }
}
