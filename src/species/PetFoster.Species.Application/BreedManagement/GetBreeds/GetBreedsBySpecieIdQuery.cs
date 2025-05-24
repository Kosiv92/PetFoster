using PetFoster.Core.Abstractions;

namespace PetFoster.Species.Application.BreedManagement.GetBreeds;

public sealed record GetBreedsBySpecieIdQuery(Guid SpecieId) : IQuery;
