using Angel.Social.Authentication.Core.Configurations;
using Angel.Social.Authentication.Extensions.Commons;
using Angel.Social.Authentication.Google.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Angel.Social.Authentication.Extensions;

public static class GoogleProviderExtensions
{
    public static IServiceCollection AddGoogleProvider(
        this IServiceCollection services,
        Action<ClientCredential> configure)
    {
        services.AddExternalProvider<IGoogleProvider, GoogleProvider>(
            configure,
            ClientCredential.GoogleKey);

        return services;
    }

    public static IServiceCollection AddGoogleProvider(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName)
    {
        services.AddExternalProvider<IGoogleProvider, GoogleProvider>(
            configuration,
            ClientCredential.GoogleKey,
            sectionName);

        return services;
    }
}
