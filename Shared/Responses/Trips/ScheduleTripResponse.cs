using Enums.Trips;
using Shared.Responses.Common;

namespace Shared.Responses.Trips;

public class ScheduleTripResponse
{
    public string PassengerEmailId { get; set; }
    public bool Office { get; set; }
    public bool OutStation { get; set; }
    public Guid PassengerUserId { get; set; }
    public VehicleTypes VehicleType { get; set; }
    public string VehicleTypeDesc { get; set; } = string.Empty;
    public AddressResponse PickUpAddress { get; set; }
    public AddressResponse DropAddress { get; set; }
    public TimeSpan DropTime { get; set; }
    public TimeSpan PickUpTime { get; set; }
    public DateTime PickUpDate { get; set; }
    public DateTime DropDate { get; set; }
    public TripStatus Status { get; set; } = TripStatus.Draft;
    public string StatusDesc { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}