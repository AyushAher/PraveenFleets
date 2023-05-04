using AutoMapper;
using Domain.Trips;
using Shared.Requests.Trips;
using Shared.Responses.Trips;
using Utility.Extensions;

namespace ApplicationServices.MappingProfile.Trips;

public class TripsProfile:Profile
{
    public TripsProfile()
    {
        CreateMap<Trip, ScheduleTripResponse>()
            .ForMember(mem=>mem.StatusDesc, des=> des.MapFrom(x=>x.Status.ToDescriptionString()))
            .ForMember(mem=>mem.VehicleTypeDesc, des=> des.MapFrom(x=>x.VehicleType.ToDescriptionString()))
            .ReverseMap();

        CreateMap<Trip, ScheduleTripRequest>()
            .ReverseMap();

        CreateMap<ScheduleTripResponse, ScheduleTripRequest>()
            .ReverseMap()
            .ForMember(mem=>mem.StatusDesc, des=> des.MapFrom(x=>x.Status.ToDescriptionString()))
            .ForMember(mem => mem.VehicleTypeDesc, des => des.MapFrom(x => x.VehicleType.ToDescriptionString()));

        CreateMap<ScheduleTripResponse, Vw_Organization_Trips>()
            .ReverseMap()
            .ForMember(mem => mem.StatusDesc, des => des.MapFrom(x => x.Status.ToDescriptionString()))
            .ForMember(mem => mem.VehicleTypeDesc, des => des.MapFrom(x => x.VehicleType.ToDescriptionString()))
            .ForMember(mem => mem.FullName, des => des.MapFrom(x => x.FirstName + " " + x.LastName));
    }
}