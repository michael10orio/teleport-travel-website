namespace Teleport.Api.Data;

public sealed class Destination
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public ICollection<Activity> Activities { get; } = new List<Activity>();
    public ICollection<ItineraryItem> ItineraryItems { get; } = new List<ItineraryItem>();
}

public sealed class Activity
{
    public long Id { get; set; }
    public long DestinationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Destination Destination { get; set; } = null!;
    public ICollection<ItineraryItem> ItineraryItems { get; } = new List<ItineraryItem>();
}

public sealed class Trip
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public ICollection<ItineraryItem> ItineraryItems { get; } = new List<ItineraryItem>();
    public ICollection<Expense> Expenses { get; } = new List<Expense>();
}

public sealed class ItineraryItem
{
    public long Id { get; set; }
    public long TripId { get; set; }
    public long? DestinationId { get; set; }
    public long? ActivityId { get; set; }
    public int SortOrder { get; set; }
    public Trip Trip { get; set; } = null!;
    public Destination? Destination { get; set; }
    public Activity? Activity { get; set; }
}

public sealed class Expense
{
    public long Id { get; set; }
    public long TripId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateOnly ExpenseDate { get; set; }
    public Trip Trip { get; set; } = null!;
}