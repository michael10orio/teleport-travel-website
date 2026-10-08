namespace Teleport.Api.Contracts;

/// <summary>Represents a destination returned by the API.</summary>
public sealed record DestinationResponse(
    long Id,
    string Name,
    string Region,
    string Category,
    string Description,
    string? ImageUrl);