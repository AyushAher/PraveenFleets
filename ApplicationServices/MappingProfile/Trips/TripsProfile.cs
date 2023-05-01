using AutoMapper;
using Domain.Trips;
using Shared.Requests.Trips;
using Shared.Responses.Trips;

namespace ApplicationServices.MappingProfile.Trips;

public class TripsProfile:Profile
{
    public TripsProfile()
    {
        CreateMap<Trip, ScheduleTripResponse>()
            .ReverseMap();

        CreateMap<Trip, ScheduleTripRequest>()
            .ReverseMap();

        CreateMap<ScheduleTripResponse, ScheduleTripRequest>()
            .ReverseMap();
    }
}