using Microsoft.AspNetCore.Http.HttpResults;
using Teleport.Api.Contracts;
using Teleport.Api.Services;

namespace Teleport.Api.Endpoints;

public static class ItineraryEndpoints
{
    public static void MapItineraryEndpoints(this WebApplication app)
    {
        var itinerary = app.MapGroup("/api/trips/{tripId:long}/itinerary-items")
            .WithTags("Itinerary");

        itinerary.MapGet("/", async Task<Results<Ok<IReadOnlyList<ItineraryItemResponse>>, NotFound>>(
            long tripId,
            IItineraryService service,
            CancellationToken cancellationToken) =>
        {
            var items = await service.GetAllAsync(tripId, cancellationToken);
            return items is null ? TypedResults.NotFound() : TypedResults.Ok(items);
        })
            .WithName("GetTripItinerary")
            .WithSummary("List a trip itinerary")
            .WithDescription("Returns itinerary items in their configured order.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        itinerary.MapPost("/", async Task<Results<Created<ItineraryItemResponse>, NotFound, ValidationProblem>>(
            long tripId,
            CreateItineraryItemRequest request,
            IItineraryService service,
            CancellationToken cancellationToken) =>
        {
            var errors = ValidateItem(request.DestinationId, request.ActivityId, request.SortOrder);
            if (errors.Count > 0)
            {
                return TypedResults.ValidationProblem(errors);
            }

            var item = await service.CreateAsync(tripId, request, cancellationToken);
            return item is null
                ? TypedResults.NotFound()
                : TypedResults.Created($"/api/trips/{tripId}/itinerary-items/{item.Id}", item);
        })
            .WithName("AddTripItineraryItem")
            .WithSummary("Add an itinerary item")
            .WithDescription("Adds one destination or activity to a trip itinerary.")
            .Produces<ItineraryItemResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        itinerary.MapPut("/{id:long}", async Task<Results<Ok<ItineraryItemResponse>, NotFound, ValidationProblem>>(
            long tripId,
            long id,
            UpdateItineraryItemRequest request,
            IItineraryService service,
            CancellationToken cancellationToken) =>
        {
            if (request.SortOrder < 0)
            {
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["sortOrder"] = ["Sort order cannot be negative."],
                });
            }

            var item = await service.UpdateAsync(tripId, id, request, cancellationToken);
            return item is null ? TypedResults.NotFound() : TypedResults.Ok(item);
        })
            .WithName("UpdateTripItineraryItem")
            .WithSummary("Change itinerary order")
            .WithDescription("Changes the display order of one itinerary item.")
            .Produces<ItineraryItemResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        itinerary.MapDelete("/{id:long}", async Task<Results<NoContent, NotFound>>(
            long tripId,
            long id,
            IItineraryService service,
            CancellationToken cancellationToken) =>
        {
            var deleted = await service.DeleteAsync(tripId, id, cancellationToken);
            return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
        })
            .WithName("DeleteTripItineraryItem")
            .WithSummary("Remove an itinerary item")
            .WithDescription("Removes one item from a trip itinerary.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static Dictionary<string, string[]> ValidateItem(
        long? destinationId,
        long? activityId,
        int sortOrder)
    {
        var errors = new Dictionary<string, string[]>();

        if (destinationId.HasValue == activityId.HasValue)
        {
            errors["destinationId"] = ["Specify exactly one destination or activity."];
        }

        if (sortOrder < 0)
        {
            errors["sortOrder"] = ["Sort order cannot be negative."];
        }

        return errors;
    }
}