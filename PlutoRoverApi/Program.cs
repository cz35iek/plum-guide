using System.Net;
using Microsoft.EntityFrameworkCore;
using PlutoRoverApi;


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Positions") ?? "Data Source=Positions.db";

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSqlite<PositionDbContext>(connectionString);
builder.Services.AddScoped<IPositionRepository, PositionRepository>();
builder.Services.AddScoped<IBoudaryService, BoudaryService>();
builder.Services.AddScoped<ICommandService, CommandService>();
builder.Services.AddScoped<IObstaclesDetector, ObstaclesDetector>();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = builder.Environment.ApplicationName, Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{builder.Environment.ApplicationName} v1"));
}

app.MapFallback(() => Results.Redirect("/swagger"));

app.MapGet("/run/{commands}", async (IPositionRepository repo, ICommandService commandService, IBoudaryService boundaries, string commands) =>
{
    // initial position
    var x = 0;
    var y = 0;
    var head = 'N';

    foreach (var command in commands)
    {
        try
        {
            (x, y, head) = commandService.Run(x, y, head, command);
        }
        catch (System.ArgumentOutOfRangeException)
        {
            return Results.BadRequest($"Unknown command {command}!");
        }
        catch (Exception)
        {
            return Results.StatusCode((int)HttpStatusCode.InternalServerError);
        }

        (x, y) = boundaries.WrapPosition(x, y);
    }

    var position = await repo.AddPosition(x, y, head);

    return Results.Ok(position);
});


app.Run();