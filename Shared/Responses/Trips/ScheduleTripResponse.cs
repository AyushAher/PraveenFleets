using Enums.Trips;
using Shared.Requests.Common;
using Shared.Responses.Common;

namespace Shared.Responses.Trips;

public class ScheduleTripResponse
{
    public string PassengerUserEmail { get; set; }
    public bool Office { get; set; }
    public bool OutStation { get; set; }
    public Guid PassengerUserId { get; set; }
    public Guid VehicleTypeId { get; set; }
    public AddressResponse PickUpAddress { get; set; }
    public AddressResponse DropAddress { get; set; }
    public TimeSpan DropTime { get; set; }
    public TimeSpan PickUpTime { get; set; }
    public DateTime PickUpDate { get; set; }
    public DateTime DropDate { get; set; }
    public TripStatus Status { get; set; } = TripStatus.Draft;
}