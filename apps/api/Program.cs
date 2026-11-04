using Microsoft.EntityFrameworkCore;
using Teleport.Api.Data;
using Teleport.Api.Endpoints;
using Teleport.Api.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("TeleportDatabase")
	?? "Host=localhost;Port=5432;Database=teleport_dev";

builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
	options.AddPolicy(
		"TeleportFrontend",
		policy => policy
			.WithOrigins("http://localhost:4200")
			.AllowAnyHeader()
			.AllowAnyMethod()));
builder.Services.AddDbContext<TeleportDbContext>(options =>
	options.UseNpgsql(connectionString));
builder.Services.AddScoped<IDestinationService, DestinationService>();
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IItineraryService, ItineraryService>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseCors("TeleportFrontend");

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.MapGet("/api/health", () => TypedResults.Ok(new { status = "ok" }))
	.WithName("GetApiHealth")
	.WithSummary("Check API health")
	.WithDescription("Returns a success response while the API host is running.")
	.Produces(StatusCodes.Status200OK);

app.MapGet("/api/health/database", async (TeleportDbContext db, CancellationToken cancellationToken) =>
{
	var canConnect = await db.Database.CanConnectAsync(cancellationToken);
	var status = canConnect ? "ok" : "unavailable";
	var statusCode = canConnect ? StatusCodes.Status200OK : StatusCodes.Status503ServiceUnavailable;
	return Results.Json(new { status }, statusCode: statusCode);
})
	.WithName("GetDatabaseHealth")
	.WithSummary("Check database connectivity")
	.WithDescription("Returns 200 when PostgreSQL accepts a connection, otherwise 503.")
	.Produces(StatusCodes.Status200OK)
	.Produces(StatusCodes.Status503ServiceUnavailable);

app.MapDestinationEndpoints();
app.MapTripEndpoints();
app.MapExpenseEndpoints();
app.MapItineraryEndpoints();

app.Run();
