using System.ComponentModel;

namespace Enums.Employee;

public enum Salutation : byte
{
    [Description("Mr.")] Mr = 10,
    [Description("Ms.")] Ms = 20,
    [Description("Mrs.")] Mrs = 30
}