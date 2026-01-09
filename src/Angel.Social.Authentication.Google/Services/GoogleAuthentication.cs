using System.Net.Http.Json;
using System.Web;
using Angel.Social.Authentication.Core.Configurations;
using Angel.Social.Authentication.Core.Exceptions;
using Angel.Social.Authentication.Core.ValueObjects;
using Angel.Social.Authentication.Google.Constants;
using Angel.Social.Authentication.Google.ValueObjects;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Angel.Social.Authentication.Google.Services;

public sealed class GoogleAuthentication(
    ILogger<GoogleAuthentication> logger,
    HttpClient httpClient,
    IOptionsSnapshot<ClientCredential> options)
    : IGoogleAuthentication
{
    private readonly ClientCredential clientCredential = options.Get(ClientCredential.GoogleKey);

    public async Task<GoogleAccessTokenResponse> AuthenticateAsync(
        SignInCode signInCode,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(signInCode);
        return await ExchangeCodeForAccessTokenAsync(signInCode, cancellationToken);
    }

    private async Task<GoogleAccessTokenResponse> ExchangeCodeForAccessTokenAsync(
        SignInCode signInCode,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Exchanging authorization code for access token.");

        var bodyRequest = new FormUrlEncodedContent([
            new(GoogleConstants.BodyKeys.Code, HttpUtility.UrlDecode(signInCode.Code)),
            new(GoogleConstants.BodyKeys.ClientId, clientCredential.ClientId),
            new(GoogleConstants.BodyKeys.ClientSecret, clientCredential.ClientSecret),
            new(GoogleConstants.BodyKeys.RedirectUri, signInCode.RedirectUri),
            new(GoogleConstants.BodyKeys.GrantType, GoogleConstants.GrantTypeAuthorizationCode)
        ]);

        var response = await httpClient.PostAsync(
            GoogleConstants.Uris.OidcTokenUrl,
            bodyRequest,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var accessToken = await response.Content.ReadFromJsonAsync<GoogleAccessTokenResponse>(cancellationToken);
        if (accessToken is null)
            throw new OAuthException("Failed to deserialize access token response.");

        logger.LogInformation("Successfully exchanged authorization code for access token.");
        return accessToken;
    }

    public Uri GetUri(GoogleOAuthRequestUrl oAuthRequest)
    {
        var queryParameters = oAuthRequest.BuildParameters(clientCredential.ClientId);

        return new Uri(QueryHelpers.AddQueryString(
            GoogleConstants.Uris.OidcAuthorizationUrl,
            queryParameters));
    }
}
