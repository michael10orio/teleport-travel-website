using Microsoft.EntityFrameworkCore;
using Teleport.Api.Contracts;
using Teleport.Api.Data;

namespace Teleport.Api.Services;

public interface ITripService
{
    Task<IReadOnlyList<TripResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<TripResponse?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<TripResponse> CreateAsync(CreateTripRequest request, CancellationToken cancellationToken);
    Task<TripResponse?> UpdateAsync(long id, UpdateTripRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken);
}

public sealed class TripService(TeleportDbContext dbContext) : ITripService
{
    public async Task<IReadOnlyList<TripResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Trips
            .AsNoTracking()
            .OrderByDescending(trip => trip.CreatedAt)
            .Select(trip => ToResponse(trip))
            .ToListAsync(cancellationToken);
    }

    public async Task<TripResponse?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var trip = await dbContext.Trips
            .AsNoTracking()
            .SingleOrDefaultAsync(trip => trip.Id == id, cancellationToken);

        return trip is null ? null : ToResponse(trip);
    }

    public async Task<TripResponse> CreateAsync(
        CreateTripRequest request,
        CancellationToken cancellationToken)
    {
        var trip = new Trip
        {
            Name = request.Name.Trim(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
        };

        dbContext.Trips.Add(trip);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(trip);
    }

    public async Task<TripResponse?> UpdateAsync(
        long id,
        UpdateTripRequest request,
        CancellationToken cancellationToken)
    {
        var trip = await dbContext.Trips
            .SingleOrDefaultAsync(trip => trip.Id == id, cancellationToken);

        if (trip is null)
        {
            return null;
        }

        trip.Name = request.Name.Trim();
        trip.StartDate = request.StartDate;
        trip.EndDate = request.EndDate;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(trip);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        var trip = await dbContext.Trips
            .SingleOrDefaultAsync(trip => trip.Id == id, cancellationToken);

        if (trip is null)
        {
            return false;
        }

        dbContext.Trips.Remove(trip);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static TripResponse ToResponse(Trip trip) => new(
        trip.Id,
        trip.Name,
        trip.StartDate,
        trip.EndDate,
        trip.CreatedAt);
}