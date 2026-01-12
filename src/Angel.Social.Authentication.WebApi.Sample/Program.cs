using System.Text.Json.Serialization;
using Angel.Social.Authentication.Core.ValueObjects;
using Angel.Social.Authentication.Extensions;
using Angel.Social.Authentication.Google.Services;
using Angel.Social.Authentication.Google.ValueObjects;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddGoogleProvider(builder.Configuration, "GoogleCredentials");

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

app.MapGet("/test", (IGoogleProvider googleAuthentication) =>
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

app.MapGet("/token", async (IGoogleProvider googleAuthentication,
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

app.MapGet("/refresh", async (IGoogleProvider googleAuthentication,
    [FromQuery] string refreshToken) =>
{
    return await googleAuthentication.RefreshAccessTokenAsync(refreshToken);
});

app.MapGet("/revoke", async (IGoogleProvider googleAuthentication,
    [FromQuery] string accessToken) =>
{
    await googleAuthentication.RevokeAccessTokenAsync(accessToken);
    return Results.Ok("Access token revoked successfully.");
});

app.MapGet("/user-info", async (IGoogleProvider googleAuthentication,
    [FromQuery] string accessToken) =>
{
    return await googleAuthentication.GetUserAsync<GooglerUser>(accessToken);
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

record GooglerUser
{
    [JsonPropertyName("sub")]
    public string Sub { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("given_name")]
    public string GivenName { get; init; } = string.Empty;

    [JsonPropertyName("family_name")]
    public string FamilyName { get; init; } = string.Empty;

    [JsonPropertyName("picture")]
    public string Picture { get; init; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;

    [JsonPropertyName("email_verified")]
    public bool EmailVerified { get; init; }
}
