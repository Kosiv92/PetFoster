using Microsoft.AspNetCore.Mvc;
using PetFoster.Application.Species.AddBreed;
using PetFoster.Application.Species.CreateSpecie;
using PetFoster.Application.Species.DeleteBreed;
using PetFoster.Application.Species.DeleteSpecie;
using PetFoster.Application.Species.GetBreeds;
using PetFoster.Application.Species.GetSpecies;
using PetFoster.WebAPI.DTO.Requests.Specie;
using PetFoster.WebAPI.DTO.Requests.Volunteer;
using PetFoster.WebAPI.Extensions;

namespace PetFoster.WebAPI.Controllers
{
    public class SpecieController : ApplicationController
    {
        [HttpGet]
        public async Task<ActionResult> GetSpecies(
            [FromQuery] GetSpeciesWithPagiationRequest request,
            [FromServices] GetSpeciesWithPaginationHandler handler,
            CancellationToken cancellationToken = default)
        {
            var query = request.ToQuery();

            var specieDtos = await handler.Handle(query, cancellationToken);

            return Ok(specieDtos);
        }

        [HttpGet("{id:guid}/breeds")]
        public async Task<ActionResult> GetSpecieBreeds([FromRoute] Guid id,            
            [FromServices] GetBreedsBySpecieIdHandler handler,
            CancellationToken cancellationToken = default)
        {
            var query = new GetBreedsBySpecieIdQuery(id);

            var breedDtos = await handler.Handle(query, cancellationToken);

            return Ok(breedDtos);
        }


        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromServices] CreateSpecieHandler handler,
            [FromBody] CreateSpecieRequest request, CancellationToken cancellationToken = default)
        {
            var command = request.ToCreateSpecieCommand(Guid.NewGuid());

            var result = await handler.Handle(command, cancellationToken);

            if (result.IsFailure)
                return result.Error.ToResponse();

            return Ok(result.Value);
        }

        [HttpPost("{id:guid}/breed")]
        public async Task<ActionResult> AddBreed([FromRoute] Guid id,
            [FromBody] AddBreedRequest request,
            [FromServices] AddBreedHandler handler,
            CancellationToken cancellationToken = default)
        {
            var command = request.ToAddBreedCommand(id);

            var result = await handler.Handle(command, cancellationToken);

            if (result.IsFailure)
                return result.Error.ToResponse();

            return Ok(result.Value);
        }



        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id,
            [FromServices] DeleteSpecieHandler handler, CancellationToken cancellationToken = default)
        {
            var command = new DeleteSpecieCommand(id);

            var result = await handler.Handle(command, cancellationToken);

            if (result.IsFailure)
                return result.Error.ToResponse();

            return Ok();
        }

        [HttpDelete("{id:guid}/breed/{breedId:guid}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id, [FromRoute] Guid breedId,
            [FromServices] DeleteBreedHandler handler, CancellationToken cancellationToken = default)
        {
            var command = new DeleteBreedCommand(id, breedId);

            var result = await handler.Handle(command, cancellationToken);

            if (result.IsFailure)
                return result.Error.ToResponse();

            return Ok();
        }
    }
}
