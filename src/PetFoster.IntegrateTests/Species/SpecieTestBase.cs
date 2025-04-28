using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Ids;
using PetFoster.Domain.Interfaces;

namespace PetFoster.IntegrateTests.Species
{
    public class SpecieTestBase
        : IClassFixture<IntegrationTestsWebFactory>, IAsyncLifetime
    {
        protected readonly IntegrationTestsWebFactory Factory;
        protected readonly Fixture Fixture;
        protected readonly IServiceScope ServiceScope;
        protected readonly IRepository<Specie, SpecieId> SpecieRepository;
        protected readonly IRepository<Volunteer, VolunteerId> VolunteerRepository;

        public SpecieTestBase(IntegrationTestsWebFactory factory)
        {
            Factory = factory;
            Fixture = new Fixture();
            ServiceScope = Factory.Services.CreateScope();
            SpecieRepository = ServiceScope.ServiceProvider
                .GetRequiredService<IRepository<Specie, SpecieId>>();
            VolunteerRepository = ServiceScope.ServiceProvider
                .GetRequiredService<IRepository<Volunteer, VolunteerId>>();
        }

        protected async Task SeedDatabase(Guid specieId, Guid breedId,
            Guid volunteerId, Guid petId,
            CancellationToken cancellationToken = default)
        {
            await SeedDatabase(specieId, new List<Guid> { breedId }, cancellationToken);

            var volunteer = Fixture.CreateVolunteer(volunteerId);
            var pet = Fixture.CreatePet(petId, specieId, breedId);
            volunteer.AddPet(pet);
            await VolunteerRepository.AddAsync(volunteer, cancellationToken);
        }

        protected async Task SeedDatabase(Guid specieId, IEnumerable<Guid> breedIds,            
            CancellationToken cancellationToken = default)
        {
            var newSpecie = Fixture.CreateSpecie(specieId);
            foreach (var id in breedIds)
            {
                var breed = Fixture.CreateBreed(id);
                newSpecie.AddBreed(breed);
            }            
            await SpecieRepository.AddAsync(newSpecie, cancellationToken);
        }

        protected async Task SeedDatabase(IEnumerable<Guid> speciesId,
            CancellationToken cancellationToken = default)
        {
            if (speciesId?.Any() == true)
            {
                foreach (var id in speciesId)
                {
                    var newSpecie = Fixture.CreateSpecie(id);
                    await SpecieRepository.AddAsync(newSpecie, cancellationToken);
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
