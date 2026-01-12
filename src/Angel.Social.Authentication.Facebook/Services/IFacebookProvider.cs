using Angel.Social.Authentication.Core.Services;
using Angel.Social.Authentication.Facebook.ValueObjects;

namespace Angel.Social.Authentication.Facebook.Services;

public interface IFacebookProvider
    : IExternalProvider<FacebookOAuthRequestUrl, FacebookAccessTokenResponse>
{
    Task<TUser> GetUserAsync<TUser>(
        string accessToken,
        string[] fields,
        CancellationToken cancellationToken = default)
        where TUser : class;
}
