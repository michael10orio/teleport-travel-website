using Teleport.Api.Contracts;

namespace Teleport.Api.Services;

public interface IDestinationService
{
    Task<IReadOnlyList<DestinationResponse>> GetAllAsync(
        string? search,
        string? category,
        CancellationToken cancellationToken);

    Task<DestinationResponse?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken);
}