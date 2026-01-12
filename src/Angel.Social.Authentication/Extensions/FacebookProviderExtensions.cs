using Angel.Social.Authentication.Core.Configurations;
using Angel.Social.Authentication.Extensions.Commons;
using Angel.Social.Authentication.Facebook.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Angel.Social.Authentication.Extensions;

public static class FacebookProviderExtensions
{
    public static IServiceCollection AddFacebookProvider(
            this IServiceCollection services,
            Action<ClientCredential> configure)
    {
        services.AddExternalProvider<IFacebookProvider, FacebookProvider>(
            configure,
            ClientCredential.FacebookKey);

        return services;
    }

    public static IServiceCollection AddFacebookProvider(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName)
    {
        services.AddExternalProvider<IFacebookProvider, FacebookProvider>(
            configuration,
            ClientCredential.FacebookKey,
            sectionName);

        return services;
    }
}
