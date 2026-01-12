namespace Angel.Social.Authentication.Facebook.Constants;

public static class FacebookConstants
{
    public const string DefaultResponseType = "code";

    public static class Uris
    {
        public const string OidcAuthorizationUrl = "https://www.facebook.com/dialog/oauth";
        public const string OidcTokenUrl = "https://graph.facebook.com/oauth/access_token";
        public const string OidcUserInfoUrl = "https://graph.facebook.com/me";
    }

    public static class QueryParameters
    {
        public const string ClientId = "client_id";
        public const string RedirectUri = "redirect_uri";
        public const string State = "state";
        public const string ResponseType = "response_type";
        public const string Scope = "scope";
        public const string AccessToken = "access_token";
        public const string Fields = "fields";
    }

    public static class BodyKeys
    {
        public const string Code = "code";
        public const string ClientId = "client_id";
        public const string ClientSecret = "client_secret";
        public const string RedirectUri = "redirect_uri";
    }
}
