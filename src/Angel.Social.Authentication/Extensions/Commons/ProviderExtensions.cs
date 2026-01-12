using Angel.Social.Authentication.Core.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Angel.Social.Authentication.Extensions.Commons;

internal static class ProviderExtensions
{
    internal static IServiceCollection AddExternalProvider<TProvider, TImplementation>(
        this IServiceCollection services,
        Action<ClientCredential> configure,
        string providerKey)
        where TProvider : class
        where TImplementation : class, TProvider
    {
        services.AddHttpClient<TProvider, TImplementation>();
        services.ConfigureCredentials(providerKey, configure);
        return services;
    }

    internal static IServiceCollection AddExternalProvider<TProvider, TImplementation>(
        this IServiceCollection services,
        IConfiguration configuration,
        string providerKey,
        string sectionName)
        where TProvider : class
        where TImplementation : class, TProvider
    {
        services.AddHttpClient<TProvider, TImplementation>();
        services.ConfigureCredentials(configuration, providerKey, sectionName);
        return services;
    }
}
