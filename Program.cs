var builder = WebApplication.CreateBuilder(args);

var socketPath = Path.Combine(Path.GetTempPath(), "jacob-webapi");

builder.WebHost.ConfigureKestrel(
    kestrel =>
    {
        kestrel.ListenLocalhost(5277);
        kestrel.ListenUnixSocket(
            socketPath,
            listen =>
            {
                var logger = listen.ApplicationServices.GetRequiredService<ILogger<Program>>();
                if (File.Exists(socketPath))
                {
                    logger.LogInformation("Our Unix Domain Socket already exists at {UdsPath}.", socketPath);
                    try
                    {
                        File.Delete(socketPath);
                        logger.LogInformation("Removed stale Unix Domain Socket at {UdsPath}.", socketPath);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(
                            e,
                            "Failed to remove stale Unix Domain Socket at {UdsPath}.",
                            socketPath
                        );
                    }
                }
            }
        );
    }
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
try
{
    app.UseSwagger();
    app.UseSwaggerUI();

    var summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    app.MapGet(
            "/weatherforecast",
            () =>
            {
                var forecast = Enumerable.Range(1, 5)
                    .Select(
                        index =>
                            new WeatherForecast(
                                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                                Random.Shared.Next(-20, 57),
                                summaries[Random.Shared.Next(summaries.Length)]
                            )
                    )
                    .ToArray();
                return forecast;
            }
        )
        .WithName("GetWeatherForecast")
        .WithOpenApi();

    app.Run();
}
finally
{
    await app.DisposeAsync();
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
