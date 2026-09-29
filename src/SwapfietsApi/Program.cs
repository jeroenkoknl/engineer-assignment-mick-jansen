var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

app.Use(async (context, next) =>
{
    var apiKey = app.Configuration["API_KEY"];

    // no key configured (e.g. local development), skip authentication
    if (string.IsNullOrEmpty(apiKey))
    {
        await next();
        return;
    }

    if (!context.Request.Headers.TryGetValue("X-Api-Key", out var provided) || provided != apiKey)
    {
        context.Response.StatusCode = 401;
        return;
    }
    await next();
});

app.MapControllers();

app.Run();
