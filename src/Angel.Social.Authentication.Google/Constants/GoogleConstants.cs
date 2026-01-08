namespace Angel.Social.Authentication.Google.Constants;

public static class GoogleConstants
{
    public const string DefaultAccessType = "offline";
    public const string DefaultResponseType = "code";
    public const string OidcAuthorizationUrl = "https://accounts.google.com/o/oauth2/v2/auth";

    public static class QueryParameters
    {
        public const string ClientId = "client_id";
        public const string RedirectUri = "redirect_uri";
        public const string ResponseType = "response_type";
        public const string Scope = "scope";
        public const string AccessType = "access_type";
        public const string IncludeGrantedScopes = "include_granted_scopes";
        public const string LoginHint = "login_hint";
        public const string Prompt = "prompt";
        public const string State = "state";
    }
}
