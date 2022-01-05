using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecyclingCalendar.Core.Interfaces;
using RecyclingCalendar.Infrastructure.RecyleApiClient;
using RecyclingCalendar.Infrastructure.Settings;

namespace RecyclingCalendar.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.AddOptions<RecyclingApiSettings>()
            .Bind(configuration.GetSection(RecyclingApiSettings.ConfigurationRootName))
            .ValidateDataAnnotations();
        services.AddSingleton<IRecyclingApiClient, RestRecyclingApiClient>();
        return services;
    }
    
}