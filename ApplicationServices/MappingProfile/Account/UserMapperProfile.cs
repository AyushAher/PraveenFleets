using AutoMapper;
using Domain.Account;
using Shared.Requests.Account;
using Shared.Responses.Account;

namespace ApplicationServices.MappingProfile.Account;

public class UserMapperProfile : Profile
{
    /// <summary>
    /// Validation rules for userMapperProfile Class
    /// </summary>
    public UserMapperProfile()
    {
        CreateMap<GetUserRequest, UserResponse>().ReverseMap();
        CreateMap<UserResponse, ApplicationUser>().ReverseMap();
    }
}