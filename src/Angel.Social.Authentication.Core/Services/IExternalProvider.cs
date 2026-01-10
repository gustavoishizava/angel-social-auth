using Angel.Social.Authentication.Core.ValueObjects;

namespace Angel.Social.Authentication.Core.Services;

public interface IExternalProvider<TOAuthRequest, TOAuthResponse>
    where TOAuthRequest : class
    where TOAuthResponse : class
{
    Task<TOAuthResponse> GetAccessTokenAsync(
        SignInCode signInCode,
        CancellationToken cancellationToken = default);

    Task<TOAuthResponse> RefreshAccessTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    Task RevokeAccessTokenAsync(
        string accessToken,
        CancellationToken cancellationToken = default);

    Uri GetUri(TOAuthRequest oAuthRequest);

    Task<TUser> GetUserAsync<TUser>(
        string accessToken,
        CancellationToken cancellationToken = default)
        where TUser : class;
}
