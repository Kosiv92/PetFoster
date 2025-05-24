namespace PetFoster.Volunteers.Application.Interfaces;

public interface IFilesCleanerService
{
    Task Process(CancellationToken cancellationToken);
}
