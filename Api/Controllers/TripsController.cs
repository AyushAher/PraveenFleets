using Interfaces.Account;
using Interfaces.Common;
using Interfaces.Trips;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Configuration;
using Shared.Requests.Trips;
using Shared.Responses.Trips;

namespace Api.Controllers;


[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TripsController : ControllerBase
{
    private readonly ITripService _tripService;


    public TripsController(
        ITripService tripService
    )
    {
        _tripService = tripService;
    }

    [HttpPost("SaveAsDraft")]
    public async Task<ApiResponse<ScheduleTripResponse>> SaveTripAsDraft(ScheduleTripRequest request)
        => await _tripService.SaveTripDraft(request);
}