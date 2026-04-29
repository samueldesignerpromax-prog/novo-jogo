using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// liberar acesso do frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();
app.UseCors("AllowAll");

var scores = new List<int>();

app.MapGet("/", () => "API do jogo rodando 🚀");

app.MapPost("/score", async (HttpContext context) =>
{
    var data = await context.Request.ReadFromJsonAsync<ScoreRequest>();

    if (data != null)
    {
        scores.Add(data.Score);
    }

    return Results.Ok(new { message = "Score salvo!" });
});

app.MapGet("/ranking", () =>
{
    return scores.OrderByDescending(s => s).Take(10);
});

var port = Environment.GetEnvironmentVariable("PORT") ?? "3000";
app.Urls.Add($"http://*:{port}");

app.Run();

record ScoreRequest(int Score);
