using Microsoft.EntityFrameworkCore;

namespace Teleport.Api.Data;

public sealed class TeleportDbContext(DbContextOptions<TeleportDbContext> options)
    : DbContext(options)
{
    public DbSet<Destination> Destinations => Set<Destination>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<Trip> Trips => Set<Trip>();
    public DbSet<ItineraryItem> ItineraryItems => Set<ItineraryItem>();
    public DbSet<Expense> Expenses => Set<Expense>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Destination>(entity =>
        {
            entity.ToTable("destinations", table =>
            {
                table.HasCheckConstraint(
                    "ck_destinations_name_not_blank",
                    "length(trim(name)) > 0");
                table.HasCheckConstraint(
                    "ck_destinations_region_not_blank",
                    "length(trim(region)) > 0");
            });
            entity.HasKey(destination => destination.Id);
            entity.Property(destination => destination.Id).HasColumnName("id");
            entity.Property(destination => destination.Name).HasColumnName("name").IsRequired();
            entity.Property(destination => destination.Region).HasColumnName("region").IsRequired();
            entity.Property(destination => destination.Category).HasColumnName("category").IsRequired();
            entity.Property(destination => destination.Description).HasColumnName("description").IsRequired();
            entity.Property(destination => destination.ImageUrl).HasColumnName("image_url");
            entity.HasIndex(destination => new { destination.Name, destination.Region })
                .IsUnique()
                .HasDatabaseName("uq_destinations_name_region");
        });

        modelBuilder.Entity<Activity>(entity =>
        {
            entity.ToTable("activities", table =>
                table.HasCheckConstraint("ck_activities_name_not_blank", "length(trim(name)) > 0"));
            entity.HasKey(activity => activity.Id);
            entity.Property(activity => activity.Id).HasColumnName("id");
            entity.Property(activity => activity.DestinationId).HasColumnName("destination_id");
            entity.Property(activity => activity.Name).HasColumnName("name").IsRequired();
            entity.Property(activity => activity.Description).HasColumnName("description").IsRequired();
            entity.HasIndex(activity => new { activity.DestinationId, activity.Name })
                .IsUnique()
                .HasDatabaseName("uq_activities_destination_name");
            entity.HasOne(activity => activity.Destination)
                .WithMany(destination => destination.Activities)
                .HasForeignKey(activity => activity.DestinationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Trip>(entity =>
        {
            entity.ToTable("trips", table =>
            {
                table.HasCheckConstraint("ck_trips_name_not_blank", "length(trim(name)) > 0");
                table.HasCheckConstraint(
                    "ck_trips_date_order",
                    "start_date IS NULL OR end_date IS NULL OR end_date >= start_date");
            });
            entity.HasKey(trip => trip.Id);
            entity.Property(trip => trip.Id).HasColumnName("id");
            entity.Property(trip => trip.Name).HasColumnName("name").IsRequired();
            entity.Property(trip => trip.StartDate).HasColumnName("start_date");
            entity.Property(trip => trip.EndDate).HasColumnName("end_date");
            entity.Property(trip => trip.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<ItineraryItem>(entity =>
        {
            entity.ToTable("itinerary_items", table =>
            {
                table.HasCheckConstraint(
                    "ck_itinerary_item_one_subject",
                    "(destination_id IS NOT NULL AND activity_id IS NULL) OR (destination_id IS NULL AND activity_id IS NOT NULL)");
                table.HasCheckConstraint("ck_itinerary_item_sort_order", "sort_order >= 0");
            });
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Id).HasColumnName("id");
            entity.Property(item => item.TripId).HasColumnName("trip_id");
            entity.Property(item => item.DestinationId).HasColumnName("destination_id");
            entity.Property(item => item.ActivityId).HasColumnName("activity_id");
            entity.Property(item => item.SortOrder)
                .HasColumnName("sort_order")
                .HasDefaultValue(0);
            entity.HasIndex(item => new { item.TripId, item.SortOrder })
                .HasDatabaseName("ix_itinerary_items_trip_order");
            entity.HasOne(item => item.Trip)
                .WithMany(trip => trip.ItineraryItems)
                .HasForeignKey(item => item.TripId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(item => item.Destination)
                .WithMany(destination => destination.ItineraryItems)
                .HasForeignKey(item => item.DestinationId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(item => item.Activity)
                .WithMany(activity => activity.ItineraryItems)
                .HasForeignKey(item => item.ActivityId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.ToTable("expenses", table =>
            {
                table.HasCheckConstraint(
                    "ck_expenses_description_not_blank",
                    "length(trim(description)) > 0");
                table.HasCheckConstraint("ck_expenses_amount_positive", "amount > 0");
            });
            entity.HasKey(expense => expense.Id);
            entity.Property(expense => expense.Id).HasColumnName("id");
            entity.Property(expense => expense.TripId).HasColumnName("trip_id");
            entity.Property(expense => expense.Description).HasColumnName("description").IsRequired();
            entity.Property(expense => expense.Amount)
                .HasColumnName("amount")
                .HasPrecision(10, 2);
            entity.Property(expense => expense.ExpenseDate)
                .HasColumnName("expense_date")
                .HasDefaultValueSql("CURRENT_DATE");
            entity.HasIndex(expense => new { expense.TripId, expense.ExpenseDate })
                .HasDatabaseName("ix_expenses_trip_date");
            entity.HasOne(expense => expense.Trip)
                .WithMany(trip => trip.Expenses)
                .HasForeignKey(expense => expense.TripId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}