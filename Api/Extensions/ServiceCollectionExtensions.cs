using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using DB.Contexts;
using Domain.Account;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Shared.Configuration;

namespace Api.Extensions;

internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add the identity Services to help in using the ASP net Identity.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <returns></returns>
    internal static IServiceCollection AddAppIdentity(
                                    this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        return serviceCollection;
    }

    internal static IServiceCollection AddJwtAuthentication(
        this IServiceCollection serviceCollection,
        AppConfiguration appConfig,
        ILogger logger)
    {
        serviceCollection
            .AddAuthentication(authentication =>
            {
                authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(bearer =>
            {
                bearer.RequireHttpsMetadata = false;
                bearer.SaveToken = true;

                bearer.TokenValidationParameters = new()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    RoleClaimType = ClaimTypes.Role,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = appConfig.JwtIssuer,
                    ValidAudience = appConfig.JwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appConfig.JwtSecurityKey))
                };
                bearer.Events = new()
                {
                    OnAuthenticationFailed = c =>
                    {
                        if (c.Exception is SecurityTokenExpiredException)
                        {
                            c.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            c.Response.ContentType = "application/json";
                            var result = JsonSerializer.Serialize(
                                ApiResponse<object>.Fail("The Token is expired.", logger));
                            return c.Response.WriteAsync(result);
                        }
#if DEBUG
                        c.NoResult();
                        c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        c.Response.ContentType = "text/plain";
                        return c.Response.WriteAsync(c.Exception.ToString());
#else
                            c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            c.Response.ContentType = "application/json";
                            var res = JsonSerializer.Serialize(
                                    ApiResponse<object>.Fail("An unhandled error has occurred.", logger));
                            return c.Response.WriteAsync(res);
#endif
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        var result = JsonSerializer.Serialize(
                            ApiResponse<object>.Fail(401,
                                "You have been logged out! Please login again.", logger));
                        return context.Response.WriteAsync(result);
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = 403;
                        context.Response.ContentType = "application/json";
                        var result = JsonSerializer.Serialize(
                            ApiResponse<object>.Fail(403, "" +
                                                  "You are not authorized to access this resource.", logger));
                        return context.Response.WriteAsync(result);
                    },
                };
            });

        serviceCollection.AddAuthorization(options =>
        {
            // Here I stored necessary permissions/roles in a constant
            foreach (var prop in typeof(PermissionsConfiguration).GetFields(
                                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            {
                options.AddPolicy(prop.GetValue(null).ToString(),
                                        policy => policy.RequireClaim(ApplicationClaimTypes.Permission,
                                                        prop.GetValue(null).ToString()));
            }
        });

        serviceCollection.AddHttpContextAccessor();
        return serviceCollection;
    }
}
