using System.Net.Http.Json;
using System.Web;
using Angel.Social.Authentication.Core.Configurations;
using Angel.Social.Authentication.Core.Exceptions;
using Angel.Social.Authentication.Core.ValueObjects;
using Angel.Social.Authentication.Facebook.Constants;
using Angel.Social.Authentication.Facebook.ValueObjects;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Angel.Social.Authentication.Facebook.Services;

public sealed class FacebookProvider(
    ILogger<FacebookProvider> logger,
    HttpClient httpClient,
    IOptionsSnapshot<ClientCredential> options) : IFacebookProvider
{
    private readonly ClientCredential clientCredential = options.Get(ClientCredential.FacebookKey);

    public Task<FacebookAccessTokenResponse> GetAccessTokenAsync(SignInCode signInCode, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(signInCode);
        return ExchangeCodeForAccessTokenAsync(signInCode, cancellationToken);
    }

    private async Task<FacebookAccessTokenResponse> ExchangeCodeForAccessTokenAsync(
        SignInCode signInCode,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Exchanging authorization code for access token.");

        List<KeyValuePair<string, string?>> queryParameters = [
            new(FacebookConstants.BodyKeys.Code, HttpUtility.UrlDecode(signInCode.Code)),
            new(FacebookConstants.BodyKeys.ClientId, clientCredential.ClientId),
            new(FacebookConstants.BodyKeys.ClientSecret, clientCredential.ClientSecret),
            new(FacebookConstants.BodyKeys.RedirectUri, signInCode.RedirectUri)
        ];

        var uri = new Uri(QueryHelpers.AddQueryString(
            FacebookConstants.Uris.OidcTokenUrl,
            queryParameters));

        var response = await httpClient.GetAsync(uri, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError(
                "Failed to exchange authorization code for access token. Status Code: {StatusCode}. Response Body: {ResponseBody}",
                response.StatusCode, errorBody);

            throw new OAuthException("Failed to exchange authorization code for access token.");
        }

        var accessToken = await response.Content.ReadFromJsonAsync<FacebookAccessTokenResponse>(cancellationToken);
        if (accessToken is null)
            throw new OAuthException("Failed to deserialize access token response.");

        return accessToken;
    }

    public Task<TUser> GetUserAsync<TUser>(string accessToken, CancellationToken cancellationToken = default) where TUser : class
    {
        throw new NotImplementedException();
    }

    public Task<FacebookAccessTokenResponse> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task RevokeAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Uri GetUri(FacebookOAuthRequestUrl oAuthRequest)
    {
        var queryParameters = oAuthRequest.BuildParameters(clientCredential.ClientId);

        return new Uri(QueryHelpers.AddQueryString(
            FacebookConstants.Uris.OidcAuthorizationUrl,
            queryParameters));
    }
}