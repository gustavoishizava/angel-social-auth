using Angel.Social.Authentication.Core.Configurations;
using Angel.Social.Authentication.Core.ValueObjects;
using Angel.Social.Authentication.Google.Constants;
using Angel.Social.Authentication.Google.ValueObjects;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Angel.Social.Authentication.Google.Services;

public sealed class GoogleAuthentication(
    ILogger<GoogleAuthentication> logger,
    HttpClient httpClient,
    [FromKeyedServices(ClientCredential.GoogleKey)] IOptions<ClientCredential> options)
    : IGoogleAuthentication
{
    public Task<AuthResponse<GoogleUser>> AuthenticateAsync(
        SignInCode signInCode,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Uri GetUri(GoogleOAuthRequestUrl oAuthRequest)
    {
        var queryParameters = oAuthRequest.BuildParameters(options.Value.ClientId);

        return new Uri(QueryHelpers.AddQueryString(
            GoogleConstants.OidcAuthorizationUrl,
            queryParameters));
    }
}
