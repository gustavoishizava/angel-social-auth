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

    public async Task<GoogleAccessTokenResponse> GetAccessTokenAsync(
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
            new(GoogleConstants.BodyKeys.GrantType, GoogleConstants.GrantTypes.AuthorizationCode)
        ]);

        return await GetAccessTokenAsync(bodyRequest, cancellationToken);
    }

    public async Task<GoogleAccessTokenResponse> RefreshAccessTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Refreshing access token.");

        var bodyRequest = new FormUrlEncodedContent([
            new(GoogleConstants.BodyKeys.RefreshToken, refreshToken),
            new(GoogleConstants.BodyKeys.ClientId, clientCredential.ClientId),
            new(GoogleConstants.BodyKeys.ClientSecret, clientCredential.ClientSecret),
            new(GoogleConstants.BodyKeys.GrantType, GoogleConstants.GrantTypes.RefreshToken)
        ]);

        return await GetAccessTokenAsync(bodyRequest, cancellationToken);
    }

    private async Task<GoogleAccessTokenResponse> GetAccessTokenAsync(
        FormUrlEncodedContent bodyRequest,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync(
            GoogleConstants.Uris.OidcTokenUrl,
            bodyRequest,
            cancellationToken);

        if (response.IsSuccessStatusCode is false)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("Failed to obtain access token. HTTP Status: {StatusCode}. Response: {Response}",
                response.StatusCode, errorBody);
            throw new OAuthException("Failed to obtain access token from Google.");
        }

        var accessToken = await response.Content.ReadFromJsonAsync<GoogleAccessTokenResponse>(cancellationToken);
        if (accessToken is null)
            throw new OAuthException("Failed to deserialize access token response.");

        logger.LogInformation("Successfully exchanged authorization code for access token.");
        return accessToken;
    }

    public async Task RevokeAccessTokenAsync(
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Revoking access token.");

        var bodyRequest = new FormUrlEncodedContent([
            new(GoogleConstants.BodyKeys.Token, accessToken)
        ]);

        var response = await httpClient.PostAsync(
            GoogleConstants.Uris.OidcRevokeTokenUrl,
            bodyRequest,
            cancellationToken);

        if (response.IsSuccessStatusCode is false)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("Failed to revoke access token. HTTP Status: {StatusCode}. Response: {Response}",
                response.StatusCode, errorBody);
            throw new OAuthException("Failed to revoke access token from Google.");
        }
    }

    public Uri GetUri(GoogleOAuthRequestUrl oAuthRequest)
    {
        var queryParameters = oAuthRequest.BuildParameters(clientCredential.ClientId);

        return new Uri(QueryHelpers.AddQueryString(
            GoogleConstants.Uris.OidcAuthorizationUrl,
            queryParameters));
    }

    public async Task<TUser> GetUserAsync<TUser>(
        string accessToken,
        CancellationToken cancellationToken = default) where TUser : class
    {
        logger.LogInformation("Obtaining user info from Google.");

        var request = new HttpRequestMessage(HttpMethod.Get, GoogleConstants.Uris.OidcUserInfoUrl);
        request.Headers.Authorization = new(GoogleConstants.TokenTypeBearer, accessToken);

        var response = await httpClient.SendAsync(request, cancellationToken);
        if (response.IsSuccessStatusCode is false)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("Failed to obtain user info. HTTP Status: {StatusCode}. Response: {Response}",
                response.StatusCode, errorBody);
            throw new OAuthException("Failed to obtain user info from Google.");
        }

        var userInfo = await response.Content.ReadFromJsonAsync<TUser>(cancellationToken);
        if (userInfo is null)
            throw new OAuthException("Failed to deserialize user info response.");

        logger.LogInformation("Successfully obtained user info.");
        return userInfo;
    }
}
