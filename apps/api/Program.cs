var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
	options.AddPolicy(
		"TeleportFrontend",
		policy => policy
			.WithOrigins("http://localhost:4200")
			.AllowAnyHeader()
			.AllowAnyMethod()));

var app = builder.Build();

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

app.Run();
