using Enums.Trips;
using Shared.Requests.Common;

namespace Shared.Requests.Trips;

public class ScheduleTripRequest
{

    public string PassengerEmailId { get; set; }
    public bool Office { get; set; }
    public bool OutStation { get; set; }
    public VehicleTypes VehicleType { get; set; }
    public AddressRequest PickUpAddress { get; set; }
    public AddressRequest DropAddress { get; set; }
    public string DropTime { get; set; }
    public string PickUpTime { get; set; }
    public DateTime PickUpDate { get; set; }
    public DateTime DropDate { get; set; }
    public TripStatus Status { get; set; } = TripStatus.Draft;
}