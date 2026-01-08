using Angel.Social.Authentication.Core.Configurations;

namespace Angel.Social.Authentication.Core.ValueObjects;

public abstract record OAuthRequest
{
    /// <summary>
    /// Determines where the API server redirects the user after the user 
    /// completes the authorization flow.
    /// </summary>
    public required string RedirectUri { get; set; }

    /// <summary>
    /// A list of scopes that identify the resources that your application 
    /// could access on the user's behalf.
    /// </summary>
    public required string[] Scopes { get; set; }

    /// <summary>
    /// Determines whether the response data included when the redirect back 
    /// to the app occurs is in URL parameters or fragments.
    /// </summary>
    public required string ResponseType { get; set; }

    /// <summary>
    /// Specifies any string value that your application uses to maintain state between 
    /// your authorization request and the authorization server's response.
    /// </summary>
    public string? State { get; set; }

    public abstract List<KeyValuePair<string, string?>> BuildParameters(string clientId);
}
