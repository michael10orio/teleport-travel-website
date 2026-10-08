namespace Teleport.Api.Contracts;

/// <summary>Represents an item in a trip itinerary.</summary>
public sealed record ItineraryItemResponse(
    long Id,
    long TripId,
    long? DestinationId,
    long? ActivityId,
    int SortOrder);

/// <summary>Payload for adding a destination or activity to a trip itinerary.</summary>
public sealed record CreateItineraryItemRequest(
    long? DestinationId,
    long? ActivityId,
    int SortOrder);

/// <summary>Payload for changing the order of an itinerary item.</summary>
public sealed record UpdateItineraryItemRequest(int SortOrder);