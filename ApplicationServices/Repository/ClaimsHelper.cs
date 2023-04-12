using System.ComponentModel;
using System.Reflection;
using System.Security.Claims;
using Domain.Account;
using Microsoft.AspNetCore.Identity;
using Shared.Configuration;
using Shared.Responses.Account;

namespace ApplicationServices.Repository;


public static class ClaimsHelper
{
    public static void GetAllPermissions(this List<RoleClaimResponse> allPermissions)
    {
        foreach (var nestedType in typeof(PermissionsConfiguration).GetNestedTypes())
        {
            var str1 = string.Empty;
            var str2 = string.Empty;

            if (nestedType.GetCustomAttributes(typeof(DisplayNameAttribute), true)
                .FirstOrDefault() is DisplayNameAttribute displayNameAttribute)
                str1 = displayNameAttribute.DisplayName;

            if (nestedType.GetCustomAttributes(typeof(DescriptionAttribute), true)
                .FirstOrDefault() is DescriptionAttribute descriptionAttribute)
                str2 = descriptionAttribute.Description;

            foreach (var field in nestedType.GetFields(BindingFlags.Static | BindingFlags.Public |
                                                       BindingFlags.FlattenHierarchy))
            {
                var obj = field.GetValue(null);
                if (obj != null)
                    allPermissions.Add(new RoleClaimResponse()
                    {
                        Value = obj.ToString(),
                        Type = "Permission",
                        Group = str1,
                        Description = str2
                    });
            }
        }
    }

    public static async Task<IdentityResult> AddPermissionClaim(
        this RoleManager<ApplicationRole> roleManager,
        ApplicationRole role,
        string permission)
    {
        return (await roleManager.GetClaimsAsync(role)).Any(a =>
            a.Type == "Permission" && a.Value == permission)
            ? IdentityResult.Failed()
            : await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
    }
}