using Enums.Trips;
using Shared.Requests.Common;

namespace Shared.Requests.Trips;

public class ScheduleTripRequest
{

    public string PassengerUserEmail { get; set; }
    public bool Office { get; set; }
    public bool OutStation { get; set; }
    public Guid VehicleTypeId { get; set; }
    public AddressRequest PickUpAddress { get; set; }
    public AddressRequest DropAddress { get; set; }
    public TimeSpan DropTime { get; set; }
    public TimeSpan PickUpTime { get; set; }
    public DateTime PickUpDate { get; set; }
    public DateTime DropDate { get; set; }
    public TripStatus Status { get; set; } = TripStatus.Draft;
}