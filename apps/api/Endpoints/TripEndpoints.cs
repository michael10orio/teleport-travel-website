using Microsoft.AspNetCore.Http.HttpResults;
using Teleport.Api.Contracts;
using Teleport.Api.Services;

namespace Teleport.Api.Endpoints;

public static class TripEndpoints
{
    public static void MapTripEndpoints(this WebApplication app)
    {
        var trips = app.MapGroup("/api/trips").WithTags("Trips");

        trips.MapGet("/", async Task<Ok<IReadOnlyList<TripResponse>>>(
            ITripService service,
            CancellationToken cancellationToken) =>
        {
            var results = await service.GetAllAsync(cancellationToken);
            return TypedResults.Ok(results);
        })
            .WithName("GetTrips")
            .WithSummary("List trips")
            .WithDescription("Returns the trips stored by the planner.")
            .Produces(StatusCodes.Status200OK);

        trips.MapGet("/{id:long}", async Task<Results<Ok<TripResponse>, NotFound>>(
            long id,
            ITripService service,
            CancellationToken cancellationToken) =>
        {
            var trip = await service.GetByIdAsync(id, cancellationToken);
            return trip is null ? TypedResults.NotFound() : TypedResults.Ok(trip);
        })
            .WithName("GetTripById")
            .WithSummary("Get a trip by ID")
            .WithDescription("Returns a trip, or 404 when it does not exist.")
            .Produces<TripResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        trips.MapPost("/", async Task<Results<Created<TripResponse>, ValidationProblem>>(
            CreateTripRequest request,
            ITripService service,
            CancellationToken cancellationToken) =>
        {
            var validationErrors = ValidateTrip(request.Name, request.StartDate, request.EndDate);
            if (validationErrors.Count > 0)
            {
                return TypedResults.ValidationProblem(validationErrors);
            }

            var trip = await service.CreateAsync(request, cancellationToken);
            return TypedResults.Created($"/api/trips/{trip.Id}", trip);
        })
            .WithName("CreateTrip")
            .WithSummary("Create a trip")
            .WithDescription("Creates a trip with a name and optional dates.")
            .Produces<TripResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        trips.MapPut("/{id:long}", async Task<Results<Ok<TripResponse>, NotFound, ValidationProblem>>(
            long id,
            UpdateTripRequest request,
            ITripService service,
            CancellationToken cancellationToken) =>
        {
            var validationErrors = ValidateTrip(request.Name, request.StartDate, request.EndDate);
            if (validationErrors.Count > 0)
            {
                return TypedResults.ValidationProblem(validationErrors);
            }

            var trip = await service.UpdateAsync(id, request, cancellationToken);
            return trip is null ? TypedResults.NotFound() : TypedResults.Ok(trip);
        })
            .WithName("UpdateTrip")
            .WithSummary("Update a trip")
            .WithDescription("Replaces the name and date range for a trip.")
            .Produces<TripResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        trips.MapDelete("/{id:long}", async Task<Results<NoContent, NotFound>>(
            long id,
            ITripService service,
            CancellationToken cancellationToken) =>
        {
            var deleted = await service.DeleteAsync(id, cancellationToken);
            return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
        })
            .WithName("DeleteTrip")
            .WithSummary("Delete a trip")
            .WithDescription("Deletes a trip and its related itinerary items and expenses.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static Dictionary<string, string[]> ValidateTrip(
        string name,
        DateOnly? startDate,
        DateOnly? endDate)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(name))
        {
            errors["name"] = ["Trip name is required."];
        }

        if (startDate.HasValue && endDate.HasValue && endDate < startDate)
        {
            errors["endDate"] = ["End date must be on or after the start date."];
        }

        return errors;
    }
}