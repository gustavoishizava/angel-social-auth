using Angel.Social.Authentication.Core.ValueObjects;

namespace Angel.Social.Authentication.Core.Services;

public interface IAuthenticationService<TOAuthRequest, TAccessTokenResponse>
    where TOAuthRequest : class
    where TAccessTokenResponse : class
{
    Task<TAccessTokenResponse> GetAccessTokenAsync(
        SignInCode signInCode,
        CancellationToken cancellationToken = default);

    Task<TAccessTokenResponse> RefreshAccessTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    Uri GetUri(TOAuthRequest oAuthRequest);
}
