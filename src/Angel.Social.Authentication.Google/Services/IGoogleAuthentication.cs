using Angel.Social.Authentication.Core.Services;
using Angel.Social.Authentication.Google.ValueObjects;

namespace Angel.Social.Authentication.Google.Services;

public interface IGoogleAuthentication
    : IAuthenticationService<GoogleOAuthRequestUrl, GoogleAccessTokenResponse>
{
}
