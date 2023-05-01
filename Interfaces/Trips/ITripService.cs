using Shared.Configuration;
using Shared.Requests.Trips;
using Shared.Responses.Trips;

namespace Interfaces.Trips;

public interface ITripService : IService
{
    Task<ApiResponse<ScheduleTripResponse>> SaveTripDraft(ScheduleTripRequest request);
}