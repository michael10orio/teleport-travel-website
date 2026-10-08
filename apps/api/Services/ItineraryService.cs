using Microsoft.EntityFrameworkCore;
using Teleport.Api.Contracts;
using Teleport.Api.Data;

namespace Teleport.Api.Services;

public interface IItineraryService
{
    Task<IReadOnlyList<ItineraryItemResponse>?> GetAllAsync(
        long tripId,
        CancellationToken cancellationToken);

    Task<ItineraryItemResponse?> CreateAsync(
        long tripId,
        CreateItineraryItemRequest request,
        CancellationToken cancellationToken);

    Task<ItineraryItemResponse?> UpdateAsync(
        long tripId,
        long id,
        UpdateItineraryItemRequest request,
        CancellationToken cancellationToken);

    Task<bool> DeleteAsync(long tripId, long id, CancellationToken cancellationToken);
}

public sealed class ItineraryService(TeleportDbContext dbContext) : IItineraryService
{
    public async Task<IReadOnlyList<ItineraryItemResponse>?> GetAllAsync(
        long tripId,
        CancellationToken cancellationToken)
    {
        if (!await dbContext.Trips.AnyAsync(trip => trip.Id == tripId, cancellationToken))
        {
            return null;
        }

        return await dbContext.ItineraryItems
            .AsNoTracking()
            .Where(item => item.TripId == tripId)
            .OrderBy(item => item.SortOrder)
            .Select(item => ToResponse(item))
            .ToListAsync(cancellationToken);
    }

    public async Task<ItineraryItemResponse?> CreateAsync(
        long tripId,
        CreateItineraryItemRequest request,
        CancellationToken cancellationToken)
    {
        if (!await dbContext.Trips.AnyAsync(trip => trip.Id == tripId, cancellationToken))
        {
            return null;
        }

        if (request.DestinationId is long destinationId
            && !await dbContext.Destinations.AnyAsync(
                destination => destination.Id == destinationId,
                cancellationToken))
        {
            return null;
        }

        if (request.ActivityId is long activityId
            && !await dbContext.Activities.AnyAsync(
                activity => activity.Id == activityId,
                cancellationToken))
        {
            return null;
        }

        var item = new ItineraryItem
        {
            TripId = tripId,
            DestinationId = request.DestinationId,
            ActivityId = request.ActivityId,
            SortOrder = request.SortOrder,
        };

        dbContext.ItineraryItems.Add(item);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(item);
    }

    public async Task<ItineraryItemResponse?> UpdateAsync(
        long tripId,
        long id,
        UpdateItineraryItemRequest request,
        CancellationToken cancellationToken)
    {
        var item = await dbContext.ItineraryItems
            .SingleOrDefaultAsync(
                item => item.TripId == tripId && item.Id == id,
                cancellationToken);

        if (item is null)
        {
            return null;
        }

        item.SortOrder = request.SortOrder;
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(item);
    }

    public async Task<bool> DeleteAsync(
        long tripId,
        long id,
        CancellationToken cancellationToken)
    {
        var item = await dbContext.ItineraryItems
            .SingleOrDefaultAsync(
                item => item.TripId == tripId && item.Id == id,
                cancellationToken);

        if (item is null)
        {
            return false;
        }

        dbContext.ItineraryItems.Remove(item);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static ItineraryItemResponse ToResponse(ItineraryItem item) => new(
        item.Id,
        item.TripId,
        item.DestinationId,
        item.ActivityId,
        item.SortOrder);
}