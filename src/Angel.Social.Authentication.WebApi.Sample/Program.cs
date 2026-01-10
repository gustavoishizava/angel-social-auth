using Angel.Social.Authentication.Core.Configurations;
using Angel.Social.Authentication.Core.ValueObjects;
using Angel.Social.Authentication.Google.Services;
using Angel.Social.Authentication.Google.ValueObjects;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHttpClient<IGoogleAuthentication, GoogleAuthentication>();
builder.Services.Configure<ClientCredential>(
    ClientCredential.GoogleKey,
    builder.Configuration.GetSection("GoogleCredentials"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/test", (IGoogleAuthentication googleAuthentication) =>
{
    var request = new GoogleOAuthRequestUrl
    {
        RedirectUri = "http://localhost:3000",
        Scopes = ["openid", "profile", "email"],
        State = "xyzABC123",
        AccessType = "offline",
        Prompt = "consent",
        ResponseType = "code"
    };
    var uri = googleAuthentication.GetUri(request);

    return uri.ToString();
})
.WithName("GetWeatherForecast");

app.MapGet("/token", async (IGoogleAuthentication googleAuthentication,
    [FromQuery] string code) =>
{
    var request = new SignInCode
    {
        Code = code,
        RedirectUri = "http://localhost:3000",
        State = "4%2F0ATX87lMfQOmiWgsHxPNOP7n-uwYsrmAI4rTunN_6Y7CL3HrMNPt47jTaZlqKcIudOwLtfQ"
    };

    return await googleAuthentication.GetAccessTokenAsync(request);
});

app.MapGet("/refresh", async (IGoogleAuthentication googleAuthentication,
    [FromQuery] string refreshToken) =>
{
    return await googleAuthentication.RefreshAccessTokenAsync(refreshToken);
});

app.MapGet("/revoke", async (IGoogleAuthentication googleAuthentication,
    [FromQuery] string accessToken) =>
{
    await googleAuthentication.RevokeAccessTokenAsync(accessToken);
    return Results.Ok("Access token revoked successfully.");
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
