using Angel.Social.Authentication.Core.Services;
using Angel.Social.Authentication.Google.ValueObjects;

namespace Angel.Social.Authentication.Google.Services;

public interface IGoogleProvider
    : IExternalProvider<GoogleOAuthRequestUrl, GoogleAccessTokenResponse>
{
}
