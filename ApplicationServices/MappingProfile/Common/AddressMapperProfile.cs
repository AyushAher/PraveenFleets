using AutoMapper;
using Domain.Common;
using Shared.Requests.Common;
using Shared.Responses.Common;

namespace ApplicationServices.MappingProfile.Common;

public class AddressMapperProfile : Profile
{
    public AddressMapperProfile()
    {
        CreateMap<AddressRequest, Address>()
            .ReverseMap();

        CreateMap<Address, AddressResponse>()
            .ReverseMap();

    }
}