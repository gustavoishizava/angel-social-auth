using Angel.Social.Authentication.Core.Services;
using Angel.Social.Authentication.Google.ValueObjects;

namespace Angel.Social.Authentication.Google.Services;

public interface IGoogleProvider
    : IExternalProvider<GoogleOAuthRequestUrl, GoogleAccessTokenResponse>
{
    Task<GoogleAccessTokenResponse> RefreshAccessTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    Task RevokeAccessTokenAsync(
        string accessToken,
        CancellationToken cancellationToken = default);

    Task<TUser> GetUserAsync<TUser>(
        string accessToken,
        CancellationToken cancellationToken = default)
        where TUser : class;
}
