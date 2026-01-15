using Angel.Social.Authentication.Core.ValueObjects;
using Angel.Social.Authentication.MercadoLivre.Constants;

namespace Angel.Social.Authentication.MercadoLivre.ValueObjects;

public sealed record MercadoLivreOAuthRequestUrl : OAuthRequest
{
    public MercadoLivreOAuthRequestUrl()
    {
        ResponseType = MercadoLivreConstants.DefaultResponseType;
    }

    public override List<KeyValuePair<string, string?>> BuildParameters(string clientId)
    {
        return
        [
            new(MercadoLivreConstants.QueryParameters.ClientId, clientId),
            new(MercadoLivreConstants.QueryParameters.RedirectUri, RedirectUri),
            new(MercadoLivreConstants.QueryParameters.ResponseType, ResponseType),
            new(MercadoLivreConstants.QueryParameters.State, State)
        ];
    }
}
