using AutoMapper;
using Domain.MasterData;
using Shared.Requests.MasterData;
using Shared.Responses.MasterData;

namespace ApplicationServices.MappingProfile.MasterData;

public class Vw_ListTypeItemsProfile : Profile
{
    public Vw_ListTypeItemsProfile()
    {
        CreateMap<Vw_ListTypeItems, Vw_ListTypeItemsResponse>()
            .ReverseMap();
        CreateMap<ListTypeItem, ListTypeItemResponse>()
            .ReverseMap();
        CreateMap<ListTypeItem, ListTypeItemRequest>()
             .ReverseMap();
    }
}