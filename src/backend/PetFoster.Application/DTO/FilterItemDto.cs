namespace PetFoster.Application.DTO;

public sealed record FilterItemDto(
    string FilterPropertyName, 
    string FilterCondition, 
    string FilterValue);