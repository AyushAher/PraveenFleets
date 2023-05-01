using Domain.Common;
using Enums.Trips;

namespace Domain.Trips;

public class Trip : EntityTemplate<Guid>
{

    // TODO: Map Foreign Keys in MySQL
    public bool Office { get; set; }
    public bool OutStation { get; set; }
    public Guid VehicleTypeId { get; set; }
    public Guid PassengerUserId { get; set; }
    public Guid PickUpAddressId { get; set; }
    public Guid DropAddressId { get; set; }
    public TimeSpan DropTime { get; set; }
    public TimeSpan PickUpTime { get; set; }
    public DateTime PickUpDate { get; set; }
    public DateTime DropDate { get; set; }
    public TripStatus Status { get; set; }
}