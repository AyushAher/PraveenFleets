#nullable enable
using System.ComponentModel;
using System.Reflection;

namespace Shared.Configuration;

/// <summary>
///   This code defines a static class called Permissions, which contains several nested static classes, each representing a different set of permissions with names like "Users", "Roles", "Audit Trails", "Documents", and "Document Types".
///   Each nested class contains a set of const string fields, with names like "View", "Create", "Edit", "Delete", "Export", and "Search", each representing a specific permission within that set.For example, the Users nested class contains the following permission strings: "Permissions.Users.View", "Permissions.Users.Create", "Permissions.Users.Edit", "Permissions.Users.Delete", and "Permissions.Users.Search".
///   The Permissions class also contains a static method called GetRegisteredPermissions(), which retrieves all of the registered permission strings by using reflection to iterate over all of the fields in all of the nested classes, and returning their string values as a List<string>.
///   Additionally, each nested class has a DisplayName and Description attribute, which provide additional information about the purpose of the permission set.These attributes can be used, for example, to display the permission sets in a UI.
/// </summary>
public static class PermissionsConfiguration
{
    public static List<string> GetRegisteredPermissions()
    {
        var permissionFields =
            typeof(PermissionsConfiguration).GetNestedTypes().SelectMany(c =>
                c.GetFields(BindingFlags.Static | BindingFlags.Public |
                            BindingFlags.FlattenHierarchy));

        return (
            from fieldInfo in permissionFields
            select fieldInfo.GetValue(null)
            into obj
            where obj != null
            select obj.ToString()).ToList();
    }

    [DisplayName("Users")]
    [Description("Users Permissions")]
    public static class Users
    {
        public const string View = "Permissions.Users.View";
        public const string Create = "Permissions.Users.Create";
        public const string Edit = "Permissions.Users.Edit";
        public const string Delete = "Permissions.Users.Delete";
        public const string Search = "Permissions.Users.Search";
    }

    [DisplayName("Roles")]
    [Description("Roles Permissions")]
    public static class Roles
    {
        public const string View = "Permissions.Roles.View";
        public const string Create = "Permissions.Roles.Create";
        public const string Edit = "Permissions.Roles.Edit";
        public const string Delete = "Permissions.Roles.Delete";
        public const string Search = "Permissions.Roles.Search";
    }

    [DisplayName("Role Claims")]
    [Description("Role Claims Permissions")]
    public static class RoleClaims
    {
        public const string View = "Permissions.RoleClaims.View";
        public const string Create = "Permissions.RoleClaims.Create";
        public const string Edit = "Permissions.RoleClaims.Edit";
        public const string Delete = "Permissions.RoleClaims.Delete";
        public const string Search = "Permissions.RoleClaims.Search";
    }

    [DisplayName("Audit Trails")]
    [Description("Audit Trails Permissions")]
    public static class AuditTrails
    {
        public const string View = "Permissions.AuditTrails.View";
        public const string Export = "Permissions.AuditTrails.Export";
        public const string Search = "Permissions.AuditTrails.Search";
    }

    [DisplayName("Documents")]
    [Description("Documents Permissions")]
    public static class Documents
    {
        public const string View = "Permissions.Documents.View";
        public const string Create = "Permissions.Documents.Create";
        public const string Edit = "Permissions.Documents.Edit";
        public const string Delete = "Permissions.Documents.Delete";
        public const string Search = "Permissions.Documents.Search";
    }

    [DisplayName("Document Types")]
    [Description("Document Types Permissions")]
    public static class DocumentTypes
    {
        public const string View = "Permissions.DocumentTypes.View";
        public const string Create = "Permissions.DocumentTypes.Create";
        public const string Edit = "Permissions.DocumentTypes.Edit";
        public const string Delete = "Permissions.DocumentTypes.Delete";
        public const string Export = "Permissions.DocumentTypes.Export";
        public const string Search = "Permissions.DocumentTypes.Search";
    }

}

public static class ApplicationClaimTypes
{
    public const string Permission = "Permission";
}

