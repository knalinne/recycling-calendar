using Microsoft.Extensions.DependencyInjection;
using RecyclingCalendar.Core.Interfaces;
using RecyclingCalendar.Core.Services;

namespace RecyclingCalendar.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddScoped<IZipCodeService, ZipCodeService>();
        services.AddScoped<IStreetService, StreetService>();
        services.AddScoped<IRecyclingEventService, RecyclingEventService>();
        return services;
    }
    
}