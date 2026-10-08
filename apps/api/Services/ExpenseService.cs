using Microsoft.EntityFrameworkCore;
using Teleport.Api.Contracts;
using Teleport.Api.Data;

namespace Teleport.Api.Services;

public interface IExpenseService
{
    Task<IReadOnlyList<ExpenseResponse>?> GetAllAsync(
        long tripId,
        CancellationToken cancellationToken);

    Task<ExpenseResponse?> GetByIdAsync(
        long tripId,
        long id,
        CancellationToken cancellationToken);

    Task<ExpenseResponse?> CreateAsync(
        long tripId,
        CreateExpenseRequest request,
        CancellationToken cancellationToken);

    Task<ExpenseResponse?> UpdateAsync(
        long tripId,
        long id,
        UpdateExpenseRequest request,
        CancellationToken cancellationToken);

    Task<bool> DeleteAsync(long tripId, long id, CancellationToken cancellationToken);
}

public sealed class ExpenseService(TeleportDbContext dbContext) : IExpenseService
{
    public async Task<IReadOnlyList<ExpenseResponse>?> GetAllAsync(
        long tripId,
        CancellationToken cancellationToken)
    {
        if (!await dbContext.Trips.AnyAsync(trip => trip.Id == tripId, cancellationToken))
        {
            return null;
        }

        return await dbContext.Expenses
            .AsNoTracking()
            .Where(expense => expense.TripId == tripId)
            .OrderByDescending(expense => expense.ExpenseDate)
            .Select(expense => ToResponse(expense))
            .ToListAsync(cancellationToken);
    }

    public async Task<ExpenseResponse?> GetByIdAsync(
        long tripId,
        long id,
        CancellationToken cancellationToken)
    {
        var expense = await dbContext.Expenses
            .AsNoTracking()
            .SingleOrDefaultAsync(
                expense => expense.TripId == tripId && expense.Id == id,
                cancellationToken);

        return expense is null ? null : ToResponse(expense);
    }

    public async Task<ExpenseResponse?> CreateAsync(
        long tripId,
        CreateExpenseRequest request,
        CancellationToken cancellationToken)
    {
        if (!await dbContext.Trips.AnyAsync(trip => trip.Id == tripId, cancellationToken))
        {
            return null;
        }

        var expense = new Expense
        {
            TripId = tripId,
            Description = request.Description.Trim(),
            Amount = request.Amount,
            ExpenseDate = request.ExpenseDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
        };

        dbContext.Expenses.Add(expense);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(expense);
    }

    public async Task<ExpenseResponse?> UpdateAsync(
        long tripId,
        long id,
        UpdateExpenseRequest request,
        CancellationToken cancellationToken)
    {
        var expense = await dbContext.Expenses
            .SingleOrDefaultAsync(
                expense => expense.TripId == tripId && expense.Id == id,
                cancellationToken);

        if (expense is null)
        {
            return null;
        }

        expense.Description = request.Description.Trim();
        expense.Amount = request.Amount;
        expense.ExpenseDate = request.ExpenseDate ?? expense.ExpenseDate;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(expense);
    }

    public async Task<bool> DeleteAsync(
        long tripId,
        long id,
        CancellationToken cancellationToken)
    {
        var expense = await dbContext.Expenses
            .SingleOrDefaultAsync(
                expense => expense.TripId == tripId && expense.Id == id,
                cancellationToken);

        if (expense is null)
        {
            return false;
        }

        dbContext.Expenses.Remove(expense);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static ExpenseResponse ToResponse(Expense expense) => new(
        expense.Id,
        expense.TripId,
        expense.Description,
        expense.Amount,
        expense.ExpenseDate);
}