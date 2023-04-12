using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Shared.Configuration;
using StackExchange.Redis;
using Utility.Email;

namespace Utility.Extensions
{
    public static class RegisterUtilsExtension
    {
        public static IServiceCollection RegisterParentUtilsServices(this IServiceCollection serviceProvider,
            ConnectionStrings connection)
        {
            serviceProvider.RegisterRedisCaching(connection);
            serviceProvider.AddSingleton(typeof(ICacheConfiguration<>), typeof(CacheConfiguration<>));
            serviceProvider.AddTransient<IMailGenerator, MailGenerator>();
            serviceProvider.AddTransient<IEMailService, SMTPMailService>();
            return serviceProvider;
        }


        public static IServiceCollection RegisterRedisCaching(this IServiceCollection serviceCollection,
            ConnectionStrings connection)
        {
            serviceCollection.AddSingleton<IConnectionMultiplexer>(opt =>
            {
                try
                {
                    var redisConnectionObj = ConnectionMultiplexer.Connect(connection.RedisCachingConnection);
                    return redisConnectionObj;

                }
                catch (Exception e)
                {
                    return null;
                }
            });
            return serviceCollection;

        }
    }

    public static class EnumExtensions
    {
        public static string ToDescriptionString(this Enum val)
        {
            var customAttributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString())
                ?.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return customAttributes == null || customAttributes.Length == 0
                ? val.ToString()
                : customAttributes[0].Description;
        }
    }
}
