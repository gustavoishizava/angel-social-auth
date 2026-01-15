using Angel.Social.Authentication.Core.ValueObjects;
using Angel.Social.Authentication.Google.Constants;

namespace Angel.Social.Authentication.Google.ValueObjects;

/// <summary>
/// Google OAuth Request Uri.
/// Documentation: https://developers.google.com/identity/protocols/oauth2/web-server
/// </summary>
public sealed record GoogleOAuthRequestUrl : OAuthRequest
{
    public const string ScopeSeparator = " ";

    /// <summary>
    /// A list of scopes that identify the resources that your application 
    /// could access on the user's behalf.
    /// </summary>
    public required string[] Scopes { get; set; } = [];

    /// <summary>
    /// Indicates whether your application can refresh access tokens when the user is not present at the browser. 
    /// Valid parameter values are online, which is the default value, and offline.
    /// </summary>
    public string? AccessType { get; set; } = GoogleConstants.DefaultAccessType;

    /// <summary>
    /// Enables applications to use incremental authorization to request access to additional scopes in context.
    /// </summary>
    public bool? IncludeGrantedScopes { get; set; }

    /// <summary>
    /// If your application knows which user is trying to authenticate, it can use this parameter to provide a 
    /// hint to the Google Authentication Server. 
    /// The server uses the hint to simplify the login flow either by prefilling the email field in the sign-in 
    /// form or by selecting the appropriate multi-login session.
    /// Set the parameter value to an email address or sub identifier, which is equivalent to the user's Google ID.
    /// </summary>
    public string? LoginHint { get; set; }

    /// <summary>
    /// A space-delimited, case-sensitive list of prompts to present the user. 
    /// If you don't specify this parameter, the user will be prompted only the first time your project requests access.
    /// </summary>
    public string? Prompt { get; set; }

    public GoogleOAuthRequestUrl()
    {
        ResponseType = GoogleConstants.DefaultResponseType;
    }

    public override List<KeyValuePair<string, string?>> BuildParameters(string clientId)
    {
        var parameters = new List<KeyValuePair<string, string?>>
        {
            new(GoogleConstants.QueryParameters.ClientId, clientId),
            new(GoogleConstants.QueryParameters.RedirectUri, RedirectUri),
            new(GoogleConstants.QueryParameters.ResponseType, ResponseType),
            new(GoogleConstants.QueryParameters.Scope, string.Join(ScopeSeparator, Scopes))
        };

        if (!string.IsNullOrEmpty(State))
            parameters.Add(new(GoogleConstants.QueryParameters.State, State!));

        if (!string.IsNullOrEmpty(AccessType))
            parameters.Add(new(GoogleConstants.QueryParameters.AccessType, AccessType!));

        if (IncludeGrantedScopes.HasValue)
        {
            parameters.Add(new(
                GoogleConstants.QueryParameters.IncludeGrantedScopes,
                IncludeGrantedScopes.Value.ToString().ToLower()));
        }

        if (!string.IsNullOrEmpty(LoginHint))
            parameters.Add(new(GoogleConstants.QueryParameters.LoginHint, LoginHint!));

        if (!string.IsNullOrEmpty(Prompt))
            parameters.Add(new(GoogleConstants.QueryParameters.Prompt, Prompt!));

        return parameters;
    }
}
