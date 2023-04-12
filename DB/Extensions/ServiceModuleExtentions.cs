using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DB.Contexts;

namespace DB.Extensions;

public static class ServiceModuleExtentions
{
    public static IServiceCollection RegisterCoreStorage(
                                                this IServiceCollection serviceCollection,
                                                        IConfiguration configuration)
    {
        serviceCollection.AddDbContext<ApplicationDbContext>(builder =>
                                GetDbContextOptions(builder, configuration));

        return serviceCollection;
    }

    public static IServiceCollection RegisterCoreInfraServices(
                                                this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddTransient(typeof(IRepositoryAsync<,>), typeof(RepositoryAsync<,>))
            .AddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));

        return serviceCollection;
    }

    private static void GetDbContextOptions(DbContextOptionsBuilder builder,
                                                        IConfiguration configuration) 
    {
        var strConnectionString = configuration.GetConnectionString("DbConnection");

        if (string.IsNullOrEmpty(strConnectionString))
            throw new ArgumentNullException(paramName: nameof(builder));

        builder.UseMySql(connectionString: strConnectionString, ServerVersion.AutoDetect(strConnectionString));
    }


}
