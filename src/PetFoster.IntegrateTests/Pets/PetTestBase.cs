using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Core.Interfaces;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Ids;

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
            Specie specie = Fixture.CreateSpecie(specieId);
            Breed breed = Fixture.CreateBreed(breedId);
            _ = specie.AddBreed(breed);
            await SpecieRepository.AddAsync(specie, cancellationToken);

            Volunteer newVolunteer = Fixture.CreateVolunteer(volunteerId);

            if (petId != null)
            {
                Pet pet = Fixture.CreatePet(petId.Value, specieId, breedId, null, null);
                _ = newVolunteer.AddPet(pet);
            }

            await VolunteerRepository.AddAsync(newVolunteer, cancellationToken);
        }

        protected async Task SeedDatabaseWithExistPets(Guid volunteerId,
            Guid specieId, Guid breedId, IEnumerable<Pet> pets,
            CancellationToken cancellationToken = default)
        {
            Specie specie = Fixture.CreateSpecie(specieId);
            Breed breed = Fixture.CreateBreed(breedId);
            _ = specie.AddBreed(breed);
            await SpecieRepository.AddAsync(specie, cancellationToken);

            Volunteer newVolunteer = Fixture.CreateVolunteer(volunteerId);

            if (pets.Any() == true)
            {
                foreach (Pet pet in pets)
                {
                    _ = newVolunteer.AddPet(pet);
                }
            }

            await VolunteerRepository.AddAsync(newVolunteer, cancellationToken);
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
