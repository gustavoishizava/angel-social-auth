using Angel.Social.Authentication.Core.Services;
using Angel.Social.Authentication.MercadoLivre.ValueObjects;

namespace Angel.Social.Authentication.MercadoLivre.Services;

public interface IMercadoLivreProvider
    : IExternalProvider<MercadoLivreOAuthRequestUrl, MercadoLivreAccessTokenResponse>
{
    Task<MercadoLivreAccessTokenResponse> RefreshAccessTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);
}
