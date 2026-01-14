using Angel.Social.Authentication.Core.Configurations;
using Angel.Social.Authentication.Extensions.Commons;
using Angel.Social.Authentication.MercadoLivre.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Angel.Social.Authentication.Extensions;

public static class MercadoLivreExtensions
{
    public static IServiceCollection AddMercadoLivreProvider(
            this IServiceCollection services,
            Action<ClientCredential> configure)
    {
        services.AddExternalProvider<IMercadoLivreProvider, MercadoLivreProvider>(
            configure,
            ClientCredential.MercadoLivreKey);

        return services;
    }

    public static IServiceCollection AddMercadoLivreProvider(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName)
    {
        services.AddExternalProvider<IMercadoLivreProvider, MercadoLivreProvider>(
            configuration,
            ClientCredential.MercadoLivreKey,
            sectionName);

        return services;
    }
}
