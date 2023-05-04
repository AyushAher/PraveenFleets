using System.Reflection;
using ApplicationServices.Account;
using ApplicationServices.Common;
using ApplicationServices.FileShare;
using ApplicationServices.MasterData;
using ApplicationServices.Organizations;
using ApplicationServices.Repository;
using ApplicationServices.Trips;
using ApplicationServices.Weatherforecast;
using Interfaces;
using Interfaces.Account;
using Interfaces.Common;
using Interfaces.FileShare;
using Interfaces.MasterData;
using Interfaces.Organizations;
using Interfaces.Trips;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Utility.Storage;

namespace ApplicationServices.Extensions;

public static class AppServiceCollectionExtensions
{
    /// <summary>
    /// Register Auto mapper
    /// </summary>
    /// <param name="services"></param>
    public static void RegisterParentAppLayer(
        this IServiceCollection services)
    {
        //Forwarded headers
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.RegisterIdentityServices();
        services.RegisterParentApplicationServices();
    }

    /// <summary>
    /// register all Application layer services
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection RegisterParentApplicationServices(
        this IServiceCollection services)
    {

        services.AddSingleton<IWeatherForecast, WeatherForecastService>();

        // Utility Services
        services.AddTransient<IFileService, FileService>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<ICurrentUserService, CurrentUserService>();
        services.AddTransient<IDocumentService, DocumentService>();

        // Common Services
        services.AddTransient<IAddressService, AddressService>();
        services.AddTransient<IMasterDataService, MasterDataService>();

        // Organization
        services.AddScoped<IOrganizationRolesService, OrganizationRoleService>();
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<IOrganizationUserService, OrganizationUserService>();
        services.AddScoped<IOrganizationEmployeeService, OrganizationEmployeeService>();


        // Trips
        services.AddTransient<ITripService, TripService>();

        return services;
    }


    public static IServiceCollection RegisterIdentityServices(this IServiceCollection services)
    {
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IRoleService, RoleService>();
        services.AddTransient<IRoleClaimService, RoleClaimService>();
        services.AddTransient<IRoleClaimsRepository, RoleClaimsRepository>();
        return services;
    }
}