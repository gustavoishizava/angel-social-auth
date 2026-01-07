using Angel.Social.Authentication.Core.ValueObjects;

namespace Angel.Social.Authentication.Core.Services;

public interface IAuthenticationService<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
    Task<AuthResponse<TResponse>> AuthenticateAsync(
        TRequest request,
        CancellationToken cancellationToken = default);
}
