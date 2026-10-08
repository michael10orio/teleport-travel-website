using Microsoft.AspNetCore.Http.HttpResults;
using Teleport.Api.Contracts;
using Teleport.Api.Services;

namespace Teleport.Api.Endpoints;

public static class DestinationEndpoints
{
    public static void MapDestinationEndpoints(this WebApplication app)
    {
        var destinations = app.MapGroup("/api/destinations")
            .WithTags("Destinations");

        destinations.MapGet("/", async Task<Ok<IReadOnlyList<DestinationResponse>>>(
            string? search,
            string? category,
            IDestinationService service,
            CancellationToken cancellationToken) =>
        {
            var results = await service.GetAllAsync(search, category, cancellationToken);
            return TypedResults.Ok(results);
        })
            .WithName("GetDestinations")
            .WithSummary("List destinations")
            .WithDescription("Lists destinations and optionally searches or filters them.")
            .Produces(StatusCodes.Status200OK);

        destinations.MapGet("/{id:long}", async Task<Results<Ok<DestinationResponse>, NotFound>>(
            long id,
            IDestinationService service,
            CancellationToken cancellationToken) =>
        {
            var destination = await service.GetByIdAsync(id, cancellationToken);
            return destination is null
                ? TypedResults.NotFound()
                : TypedResults.Ok(destination);
        })
            .WithName("GetDestinationById")
            .WithSummary("Get a destination by ID")
            .WithDescription("Returns one destination, or 404 when it does not exist.")
            .Produces<DestinationResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }
}