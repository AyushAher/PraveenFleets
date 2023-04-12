using System.ComponentModel;

namespace Enums.Account;


public enum PrimaryRoles : byte
{
    [Description("Admin")] AdminRole = 10,
    [Description("SuperAdmin")] SuperAdminRole = 20,
    [Description("Base")] Base = 20,
}