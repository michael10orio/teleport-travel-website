using Microsoft.EntityFrameworkCore;
using Teleport.Api.Contracts;
using Teleport.Api.Data;

namespace Teleport.Api.Services;

public sealed class DestinationService(TeleportDbContext dbContext) : IDestinationService
{
    public async Task<IReadOnlyList<DestinationResponse>> GetAllAsync(
        string? search,
        string? category,
        CancellationToken cancellationToken)
    {
        IQueryable<Destination> destinations = dbContext.Destinations.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = $"%{search.Trim()}%";
            destinations = destinations.Where(destination =>
                EF.Functions.ILike(destination.Name, searchTerm)
                || EF.Functions.ILike(destination.Region, searchTerm)
                || EF.Functions.ILike(destination.Description, searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            var categoryTerm = category.Trim();
            destinations = destinations.Where(destination =>
                EF.Functions.ILike(destination.Category, categoryTerm));
        }

        return await destinations
            .OrderBy(destination => destination.Name)
            .Select(destination => new DestinationResponse(
                destination.Id,
                destination.Name,
                destination.Region,
                destination.Category,
                destination.Description,
                destination.ImageUrl))
            .ToListAsync(cancellationToken);
    }

    public async Task<DestinationResponse?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken)
    {
        return await dbContext.Destinations
            .AsNoTracking()
            .Where(destination => destination.Id == id)
            .Select(destination => new DestinationResponse(
                destination.Id,
                destination.Name,
                destination.Region,
                destination.Category,
                destination.Description,
                destination.ImageUrl))
            .SingleOrDefaultAsync(cancellationToken);
    }
}