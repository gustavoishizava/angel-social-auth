using Angel.Social.Authentication.Core.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Angel.Social.Authentication.Extensions.Commons;

internal static class ClientCredentialExtensions
{
    internal static IServiceCollection ConfigureCredentials(
            this IServiceCollection services,
            string providerKey,
            Action<ClientCredential> configure)
    {
        services.Configure(providerKey, configure);
        return services;
    }

    internal static IServiceCollection ConfigureCredentials(
        this IServiceCollection services,
        IConfiguration configuration,
        string providerKey,
        string sectionName)
    {
        services.Configure<ClientCredential>(
            providerKey,
            configuration.GetSection(sectionName));

        return services;
    }
}
