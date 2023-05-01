using System.ComponentModel;

namespace Enums.Trips;

public enum TripStatus : byte
{
    [Description("Draft")] Draft = 10,
    [Description("Waiting")] Waiting = 20,
    [Description("Approved")] Approved = 30,
    [Description("Canceled")] Canceled = 40,
    [Description("Declined")] Declined = 50,

}