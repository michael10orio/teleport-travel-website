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
        var destination = await dbContext.Destinations
            .AsNoTracking()
            .SingleOrDefaultAsync(destination => destination.Id == id, cancellationToken);

        return destination is null
            ? null
            : new DestinationResponse(
                destination.Id,
                destination.Name,
                destination.Region,
                destination.Category,
                destination.Description,
                destination.ImageUrl);
    }

    public async Task<IReadOnlyList<ActivityResponse>?> GetActivitiesAsync(
        long destinationId,
        CancellationToken cancellationToken)
    {
        if (!await dbContext.Destinations.AnyAsync(
                destination => destination.Id == destinationId,
                cancellationToken))
        {
            return null;
        }

        return await dbContext.Activities
            .AsNoTracking()
            .Where(activity => activity.DestinationId == destinationId)
            .OrderBy(activity => activity.Name)
            .Select(activity => new ActivityResponse(
                activity.Id,
                activity.DestinationId,
                activity.Name,
                activity.Description))
            .ToListAsync(cancellationToken);
    }
}