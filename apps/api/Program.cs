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

app.MapDestinationEndpoints();

app.Run();
