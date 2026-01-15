using System.Net.Http.Json;
using System.Web;
using Angel.Social.Authentication.Core.Configurations;
using Angel.Social.Authentication.Core.Exceptions;
using Angel.Social.Authentication.Core.ValueObjects;
using Angel.Social.Authentication.MercadoLivre.Constants;
using Angel.Social.Authentication.MercadoLivre.ValueObjects;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Angel.Social.Authentication.MercadoLivre.Services;

public sealed class MercadoLivreProvider(
    ILogger<MercadoLivreProvider> logger,
    HttpClient httpClient,
    IOptionsSnapshot<ClientCredential> clientCredential) : IMercadoLivreProvider
{
    private readonly ClientCredential clientCredential =
        clientCredential.Get(ClientCredential.MercadoLivreKey);

    public async Task<MercadoLivreAccessTokenResponse> GetAccessTokenAsync(
        SignInCode signInCode,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(signInCode);
        return await ExchangeCodeForAccessTokenAsync(signInCode, cancellationToken);
    }

    private async Task<MercadoLivreAccessTokenResponse> ExchangeCodeForAccessTokenAsync(
        SignInCode signInCode,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Exchanging authorization code for access token.");

        var content = new FormUrlEncodedContent([
            new(MercadoLivreConstants.BodyKeys.Code, HttpUtility.UrlDecode(signInCode.Code)),
            new(MercadoLivreConstants.BodyKeys.ClientId, clientCredential.ClientId),
            new(MercadoLivreConstants.BodyKeys.ClientSecret, clientCredential.ClientSecret),
            new(MercadoLivreConstants.BodyKeys.RedirectUri, signInCode.RedirectUri),
            new(MercadoLivreConstants.BodyKeys.GrantType, MercadoLivreConstants.GrantTypes.AuthorizationCode)
        ]);

        return await GetAccessTokenAsync(content, cancellationToken);
    }

    public async Task<MercadoLivreAccessTokenResponse> RefreshAccessTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Refreshing access token.");

        var bodyRequest = new FormUrlEncodedContent([
            new(MercadoLivreConstants.BodyKeys.RefreshToken, refreshToken),
            new(MercadoLivreConstants.BodyKeys.ClientId, clientCredential.ClientId),
            new(MercadoLivreConstants.BodyKeys.ClientSecret, clientCredential.ClientSecret),
            new(MercadoLivreConstants.BodyKeys.GrantType, MercadoLivreConstants.GrantTypes.RefreshToken)
        ]);

        return await GetAccessTokenAsync(bodyRequest, cancellationToken);
    }

    private async Task<MercadoLivreAccessTokenResponse> GetAccessTokenAsync(
        FormUrlEncodedContent bodyRequest,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync(
            MercadoLivreConstants.Uris.OidcTokenUrl,
            bodyRequest,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError(
                "Failed to exchange authorization code for access token. Status Code: {StatusCode}. Response Body: {ResponseBody}",
                response.StatusCode, errorBody);

            throw new OAuthException("Failed to exchange authorization code for access token.");
        }

        var accessToken = await response.Content.ReadFromJsonAsync<MercadoLivreAccessTokenResponse>(cancellationToken);
        if (accessToken is null)
            throw new OAuthException("Failed to deserialize access token response.");

        return accessToken;
    }

    public Uri GetUri(MercadoLivreOAuthRequestUrl oAuthRequest)
    {
        var queryParameters = oAuthRequest.BuildParameters(clientCredential.ClientId);

        return new Uri(QueryHelpers.AddQueryString(
            MercadoLivreConstants.Uris.OidcAuthorizationUrl,
            queryParameters));
    }
}