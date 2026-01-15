using Angel.Social.Authentication.Core.ValueObjects;
using Angel.Social.Authentication.Facebook.Constants;

namespace Angel.Social.Authentication.Facebook.ValueObjects;

/// <summary>
/// Facebook OAuth Request Uri.
/// Documentation: https://developers.facebook.com/docs/facebook-login/guides/advanced/manual-flow
/// </summary>
public sealed record FacebookOAuthRequestUrl : OAuthRequest
{
    public const string ScopeSeparator = ",";

    /// <summary>
    /// A list of scopes that identify the resources that your application 
    /// could access on the user's behalf.
    /// </summary>
    public required string[] Scopes { get; set; } = [];

    public FacebookOAuthRequestUrl()
    {
        ResponseType = FacebookConstants.DefaultResponseType;
    }

    public override List<KeyValuePair<string, string?>> BuildParameters(string clientId)
    {
        return
        [
            new(FacebookConstants.QueryParameters.ClientId, clientId),
            new(FacebookConstants.QueryParameters.RedirectUri, RedirectUri),
            new(FacebookConstants.QueryParameters.ResponseType, ResponseType),
            new(FacebookConstants.QueryParameters.Scope, string.Join(ScopeSeparator, Scopes)),
            new(FacebookConstants.QueryParameters.State, State)
        ];
    }
}
