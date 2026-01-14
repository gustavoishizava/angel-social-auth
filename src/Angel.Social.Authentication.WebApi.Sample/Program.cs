using System.Text.Json.Serialization;
using Angel.Social.Authentication.Core.ValueObjects;
using Angel.Social.Authentication.Extensions;
using Angel.Social.Authentication.Facebook.Services;
using Angel.Social.Authentication.Facebook.ValueObjects;
using Angel.Social.Authentication.Google.Services;
using Angel.Social.Authentication.Google.ValueObjects;
using Angel.Social.Authentication.MercadoLivre.Services;
using Angel.Social.Authentication.MercadoLivre.ValueObjects;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddGoogleProvider(builder.Configuration, "GoogleCredentials");
builder.Services.AddFacebookProvider(builder.Configuration, "FacebookCredentials");
builder.Services.AddMercadoLivreProvider(builder.Configuration, "MercadoLivreCredentials");

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

app.MapGet("/{provider}/auth-uri", (IGoogleProvider googleProvider,
    IFacebookProvider facebookProvider,
    IMercadoLivreProvider mercadoLivreProvider,
    [FromRoute] string provider) =>
{
    Uri? uri = null;

    switch (provider.ToLower())
    {
        case "google":
            var request = new GoogleOAuthRequestUrl
            {
                RedirectUri = "http://localhost:3000",
                Scopes = ["openid", "profile", "email"],
                State = "xyzABC123",
                AccessType = "offline",
                Prompt = "consent",
                ResponseType = "code"
            };
            uri = googleProvider.GetUri(request);
            break;
        case "facebook":
            var facebookRequest = new FacebookOAuthRequestUrl
            {
                RedirectUri = "https://localhost:3000/sign-in",
                Scopes = ["email", "public_profile", "email"],
                State = "xyzABC123",
                ResponseType = "code"
            };
            uri = facebookProvider.GetUri(facebookRequest);
            break;
        case "mercadolivre":
            var mercadoLivreRequest = new MercadoLivreOAuthRequestUrl
            {
                RedirectUri = "https://red-pebble-00508c50f.4.azurestaticapps.net",
                State = "xyzABC123",
                ResponseType = "code"
            };
            uri = mercadoLivreProvider.GetUri(mercadoLivreRequest);
            break;
    }

    return uri is not null
        ? Results.Ok(uri.ToString())
        : Results.BadRequest("Unsupported provider.");
})
.WithName("Get AuthUri");

app.MapGet("/{provider}/token", async (IGoogleProvider googleProvider,
    IFacebookProvider facebookProvider,
    IMercadoLivreProvider mercadoLivreProvider,
    [FromRoute] string provider,
    [FromQuery] string code) =>
{
    var request = new SignInCode
    {
        Code = code,
        RedirectUri = "https://red-pebble-00508c50f.4.azurestaticapps.net",
        State = "4%2F0ATX87lMfQOmiWgsHxPNOP7n-uwYsrmAI4rTunN_6Y7CL3HrMNPt47jTaZlqKcIudOwLtfQ"
    };

    switch (provider.ToLower())
    {
        case "google":
            return Results.Ok(await googleProvider.GetAccessTokenAsync(request));
        case "facebook":
            return Results.Ok(await facebookProvider.GetAccessTokenAsync(request));
        case "mercadolivre":
            return Results.Ok(await mercadoLivreProvider.GetAccessTokenAsync(request));
        default:
            return Results.BadRequest("Unsupported provider.");
    }
}).WithName("Get AccessToken");

app.MapGet("/{provider}/refresh", async (IGoogleProvider googleProvider,
    IMercadoLivreProvider mercadoLivreProvider,
    [FromRoute] string provider,
    [FromQuery] string refreshToken) =>
{
    switch (provider.ToLower())
    {
        case "google":
            return Results.Ok(await googleProvider.RefreshAccessTokenAsync(refreshToken));
        case "mercadolivre":
            return Results.Ok(await mercadoLivreProvider.RefreshAccessTokenAsync(refreshToken));
        default:
            return Results.BadRequest("Unsupported provider.");
    }
});

app.MapGet("/revoke", async (IGoogleProvider googleAuthentication,
    [FromQuery] string accessToken) =>
{
    await googleAuthentication.RevokeAccessTokenAsync(accessToken);
    return Results.Ok("Access token revoked successfully.");
});

app.MapGet("/{provider}/user-info", async (IGoogleProvider googleAuthentication,
    IFacebookProvider facebookAuthentication,
    [FromRoute] string provider,
    [FromQuery] string accessToken) =>
{
    return provider.ToLower() switch
    {
        "google" => Results.Ok(await googleAuthentication.GetUserAsync<GooglerUser>(accessToken)),
        "facebook" => Results.Ok(await facebookAuthentication.GetUserAsync<FacebookUser>(accessToken, new[] { "id", "name", "email" })),
        _ => Results.BadRequest("Unsupported provider."),
    };
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

record FacebookUser
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;
}
