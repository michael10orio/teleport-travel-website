using Microsoft.AspNetCore.Http.HttpResults;
using Teleport.Api.Contracts;
using Teleport.Api.Services;

namespace Teleport.Api.Endpoints;

public static class ExpenseEndpoints
{
    public static void MapExpenseEndpoints(this WebApplication app)
    {
        var expenses = app.MapGroup("/api/trips/{tripId:long}/expenses")
            .WithTags("Expenses");

        expenses.MapGet("/", async Task<Results<Ok<IReadOnlyList<ExpenseResponse>>, NotFound>>(
            long tripId,
            IExpenseService service,
            CancellationToken cancellationToken) =>
        {
            var results = await service.GetAllAsync(tripId, cancellationToken);
            return results is null ? TypedResults.NotFound() : TypedResults.Ok(results);
        })
            .WithName("GetTripExpenses")
            .WithSummary("List expenses for a trip")
            .WithDescription("Returns all expenses associated with the trip.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        expenses.MapGet("/{id:long}", async Task<Results<Ok<ExpenseResponse>, NotFound>>(
            long tripId,
            long id,
            IExpenseService service,
            CancellationToken cancellationToken) =>
        {
            var expense = await service.GetByIdAsync(tripId, id, cancellationToken);
            return expense is null ? TypedResults.NotFound() : TypedResults.Ok(expense);
        })
            .WithName("GetTripExpenseById")
            .WithSummary("Get a trip expense by ID")
            .WithDescription("Returns one expense associated with the trip.")
            .Produces<ExpenseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        expenses.MapPost("/", async Task<Results<Created<ExpenseResponse>, NotFound, ValidationProblem>>(
            long tripId,
            CreateExpenseRequest request,
            IExpenseService service,
            CancellationToken cancellationToken) =>
        {
            var errors = ValidateExpense(request.Description, request.Amount);
            if (errors.Count > 0)
            {
                return TypedResults.ValidationProblem(errors);
            }

            var expense = await service.CreateAsync(tripId, request, cancellationToken);
            return expense is null
                ? TypedResults.NotFound()
                : TypedResults.Created($"/api/trips/{tripId}/expenses/{expense.Id}", expense);
        })
            .WithName("CreateTripExpense")
            .WithSummary("Add an expense to a trip")
            .WithDescription("Creates an expense with a positive amount and description.")
            .Produces<ExpenseResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        expenses.MapPut("/{id:long}", async Task<Results<Ok<ExpenseResponse>, NotFound, ValidationProblem>>(
            long tripId,
            long id,
            UpdateExpenseRequest request,
            IExpenseService service,
            CancellationToken cancellationToken) =>
        {
            var errors = ValidateExpense(request.Description, request.Amount);
            if (errors.Count > 0)
            {
                return TypedResults.ValidationProblem(errors);
            }

            var expense = await service.UpdateAsync(tripId, id, request, cancellationToken);
            return expense is null ? TypedResults.NotFound() : TypedResults.Ok(expense);
        })
            .WithName("UpdateTripExpense")
            .WithSummary("Update a trip expense")
            .WithDescription("Replaces the description, amount, and optional date of an expense.")
            .Produces<ExpenseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        expenses.MapDelete("/{id:long}", async Task<Results<NoContent, NotFound>>(
            long tripId,
            long id,
            IExpenseService service,
            CancellationToken cancellationToken) =>
        {
            var deleted = await service.DeleteAsync(tripId, id, cancellationToken);
            return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
        })
            .WithName("DeleteTripExpense")
            .WithSummary("Delete a trip expense")
            .WithDescription("Deletes one expense associated with the trip.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static Dictionary<string, string[]> ValidateExpense(string description, decimal amount)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(description))
        {
            errors["description"] = ["Expense description is required."];
        }

        if (amount <= 0)
        {
            errors["amount"] = ["Expense amount must be greater than zero."];
        }

        return errors;
    }
}