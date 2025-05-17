using Microsoft.AspNetCore.Mvc;
using PetFoster.Core.DTO.Specie;
using PetFoster.Species.Application.BreedManagement.AddBreed;
using PetFoster.Species.Application.BreedManagement.DeleteBreed;
using PetFoster.Species.Application.BreedManagement.GetBreeds;
using PetFoster.Species.Application.SpecieManagement.CreateSpecie;
using PetFoster.Species.Application.SpecieManagement.DeleteSpecie;
using PetFoster.Species.Application.SpecieManagement.GetSpecies;
using PetFoster.Species.Controllers.DTO.Requests.Specie;
using PetFoster.WebAPI.Controllers;
using PetFoster.WebAPI.Extensions;

namespace PetFoster.Species.Controllers
{
    public class SpecieController : ApplicationController
    {
        [HttpGet]
        public async Task<ActionResult> GetSpecies(
            [FromQuery] GetSpeciesWithPagiationRequest request,
            [FromServices] GetSpeciesWithPaginationHandler handler,
            CancellationToken cancellationToken = default)
        {
            GetSpeciesWithPaginationQuery query = request.ToQuery();

            Core.PagedList<SpecieDto> specieDtos = await handler.Handle(query, cancellationToken);

            return Ok(specieDtos);
        }

        [HttpGet("{id:guid}/breeds")]
        public async Task<ActionResult> GetSpecieBreeds([FromRoute] Guid id,
            [FromServices] GetBreedsBySpecieIdHandler handler,
            CancellationToken cancellationToken = default)
        {
            GetBreedsBySpecieIdQuery query = new(id);

            List<BreedDto> breedDtos = await handler.Handle(query, cancellationToken);

            return Ok(breedDtos);
        }


        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromServices] CreateSpecieHandler handler,
            [FromBody] CreateSpecieRequest request, CancellationToken cancellationToken = default)
        {
            CreateSpecieCommand command = request.ToCreateSpecieCommand(Guid.NewGuid());

            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> result 
                = await handler.Handle(command, cancellationToken);

            return result.IsFailure 
                ? (ActionResult<Guid>)result.Error.ToResponse() 
                : (ActionResult<Guid>)Ok(result.Value);
        }

        [HttpPost("{id:guid}/breed")]
        public async Task<ActionResult> AddBreed([FromRoute] Guid id,
            [FromBody] AddBreedRequest request,
            [FromServices] AddBreedHandler handler,
            CancellationToken cancellationToken = default)
        {
            AddBreedCommand command = request.ToAddBreedCommand(id);

            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> result 
                = await handler.Handle(command, cancellationToken);

            return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
        }



        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id,
            [FromServices] DeleteSpecieHandler handler, CancellationToken cancellationToken = default)
        {
            DeleteSpecieCommand command = new(id);

            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> result 
                = await handler.Handle(command, cancellationToken);

            return result.IsFailure ? result.Error.ToResponse() : Ok();
        }

        [HttpDelete("{id:guid}/breed/{breedId:guid}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id, [FromRoute] Guid breedId,
            [FromServices] DeleteBreedHandler handler, CancellationToken cancellationToken = default)
        {
            DeleteBreedCommand command = new(id, breedId);

            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> result 
                = await handler.Handle(command, cancellationToken);

            return result.IsFailure ? result.Error.ToResponse() : Ok();
        }
    }
}
