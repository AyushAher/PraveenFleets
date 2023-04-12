using AutoMapper;
using Domain.Account;
using Shared.Responses.Account;

namespace ApplicationServices.MappingProfile.Account;

public class RoleProfile : Profile
{
    public RoleProfile() => CreateMap<RoleResponse, ApplicationRole>().ReverseMap();

}