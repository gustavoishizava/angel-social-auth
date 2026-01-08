using Angel.Social.Authentication.Core.ValueObjects;

namespace Angel.Social.Authentication.Core.Services;

public interface IAuthenticationService<TOAuthRequest, TResponse>
    where TOAuthRequest : class
    where TResponse : class
{
    Task<AuthResponse<TResponse>> AuthenticateAsync(
        SignInCode signInCode,
        CancellationToken cancellationToken = default);

    Uri GetUri(TOAuthRequest oAuthRequest);
}
