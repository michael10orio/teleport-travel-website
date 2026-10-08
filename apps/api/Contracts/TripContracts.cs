namespace Teleport.Api.Contracts;

/// <summary>Represents a trip returned by the API.</summary>
public sealed record TripResponse(
    long Id,
    string Name,
    DateOnly? StartDate,
    DateOnly? EndDate,
    DateTimeOffset CreatedAt);

/// <summary>Payload for creating a trip.</summary>
public sealed record CreateTripRequest(
    string Name,
    DateOnly? StartDate,
    DateOnly? EndDate);

/// <summary>Payload for updating a trip.</summary>
public sealed record UpdateTripRequest(
    string Name,
    DateOnly? StartDate,
    DateOnly? EndDate);