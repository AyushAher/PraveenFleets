using AutoMapper;
using Domain.MasterData;
using Shared.Requests.MasterData;
using Shared.Responses.MasterData;

namespace ApplicationServices.MappingProfile.MasterData;

public class ListTypeItemProfile : Profile
{
    public ListTypeItemProfile()
    {
        CreateMap<ListTypeItem, ListTypeItemResponse>()
            .ReverseMap();
        CreateMap<ListTypeItem, ListTypeItemRequest>()
            .ReverseMap();
        CreateMap<ListTypeItemResponse, ListTypeItemRequest>()
            .ReverseMap();
        CreateMap<Vw_ListTypeItemsResponse, ListTypeItem>()
            .ReverseMap();
    }

}