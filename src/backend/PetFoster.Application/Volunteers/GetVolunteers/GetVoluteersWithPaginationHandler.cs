﻿using Microsoft.Extensions.Logging;
using PetFoster.Application.DTO.Volunteer;
using PetFoster.Application.Interfaces;
using PetFoster.Domain.Interfaces;
using PetFoster.Domain.Shared;

namespace PetFoster.Application.Volunteers.GetVolunteers
{
    public sealed class GetVoluteersWithPaginationHandler 
        : IQueryHandler<PagedList<VolunteerDto>, GetVoluteersWithPaginationQuery>
    {
        private readonly IVolunteersQueryRepository _repository;
        private readonly ILogger<GetVoluteersWithPaginationHandler> _logger;

        public GetVoluteersWithPaginationHandler(IVolunteersQueryRepository repository, 
            ILogger<GetVoluteersWithPaginationHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<PagedList<VolunteerDto>> Handle(GetVoluteersWithPaginationQuery query, CancellationToken cancellationToken = default)
        {
            var volunteers = await _repository.GetAllAsync(query, cancellationToken);
            
            _logger.LogInformation("Get {VolunteersCount} volunteers with query {@query}", 
                volunteers?.Items.Count, query);

            return volunteers;
        }
    }
}
