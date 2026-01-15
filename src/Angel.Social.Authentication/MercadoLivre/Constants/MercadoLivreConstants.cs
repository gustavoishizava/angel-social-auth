namespace Angel.Social.Authentication.MercadoLivre.Constants;

public static class MercadoLivreConstants
{
    public const string DefaultResponseType = "code";

    public static class Uris
    {
        public const string OidcAuthorizationUrl = "https://auth.mercadolivre.com.br/authorization";
        public const string OidcTokenUrl = "https://api.mercadolibre.com/oauth/token";
    }

    public static class GrantTypes
    {
        public const string AuthorizationCode = "authorization_code";
        public const string RefreshToken = "refresh_token";
    }

    public static class QueryParameters
    {
        public const string ClientId = "client_id";
        public const string RedirectUri = "redirect_uri";
        public const string State = "state";
        public const string ResponseType = "response_type";
    }

    public static class BodyKeys
    {
        public const string Code = "code";
        public const string ClientId = "client_id";
        public const string ClientSecret = "client_secret";
        public const string RedirectUri = "redirect_uri";
        public const string GrantType = "grant_type";
        public const string RefreshToken = "refresh_token";
    }
}
