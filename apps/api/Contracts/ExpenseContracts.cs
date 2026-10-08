namespace Teleport.Api.Contracts;

/// <summary>Represents an expense returned by the API.</summary>
public sealed record ExpenseResponse(
    long Id,
    long TripId,
    string Description,
    decimal Amount,
    DateOnly ExpenseDate);

/// <summary>Payload for creating an expense.</summary>
public sealed record CreateExpenseRequest(
    string Description,
    decimal Amount,
    DateOnly? ExpenseDate);

/// <summary>Payload for updating an expense.</summary>
public sealed record UpdateExpenseRequest(
    string Description,
    decimal Amount,
    DateOnly? ExpenseDate);