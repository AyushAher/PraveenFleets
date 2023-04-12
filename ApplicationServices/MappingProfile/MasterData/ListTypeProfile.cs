using AutoMapper;
using Domain.MasterData;
using Shared.Requests.MasterData;
using Shared.Responses.MasterData;

namespace ApplicationServices.MappingProfile.MasterData;

public class ListTypeProfile : Profile
{
    public ListTypeProfile()
    {
        CreateMap<ListType, ListTypeResponse>()
            .ReverseMap();

        CreateMap<ListTypeRequest, ListTypeResponse>()
            .ReverseMap();

        CreateMap<ListTypeRequest, ListType>()
            .ReverseMap();
    }
}