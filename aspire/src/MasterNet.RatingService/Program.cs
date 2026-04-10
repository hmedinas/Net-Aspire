using System.Diagnostics;
using MasterNet.RatingService;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddCors();

builder.AddRedisClient(connectionName: "cache");

builder.Services.AddSingleton<RatingStore>();

var app = builder.Build();

app.UseCors(x => x.AllowAnyOrigin());


app.MapPost("/ratings", ([FromBody]RatingRequest request, RatingStore ratingStore) =>
{
    Activity.Current?.AddEvent(new ActivityEvent("Evento post"));
    if (string.IsNullOrWhiteSpace(request.Id))
    {
        return Results.BadRequest("Ingrese un valido curso Id");
    }

    ratingStore.AddRating(request.Id, request.Rating);

    Activity.Current?.SetTag("CourseId", request.Id);
    Activity.Current?.SetTag("rating", request.Rating);


    return Results.Ok();

});

app.MapGet("/ratings", async ([FromQuery]string id, RatingStore ratingStore) =>
{
    if (string.IsNullOrWhiteSpace(id))
    {
        return Results.BadRequest("Ingrese un valor de curso Id");
    }

    var rating = await ratingStore.GetAverageRating(id);

    Activity.Current?.SetTag("CourseId", id);
    Activity.Current?.SetTag("rating", rating);

    return Results.Ok(rating);
});



app.MapDefaultEndpoints();

app.Run();

public record RatingRequest(string Id, int Rating);
