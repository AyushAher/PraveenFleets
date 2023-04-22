using AutoMapper;
using Domain.Organization;
using Shared.Responses.Organization;

namespace ApplicationServices.MappingProfile.Organizations;

public class OrganizationRoleProfile : Profile
{
    public OrganizationRoleProfile()
    {
        CreateMap<Vw_OrganizationRoles, OrganizationRoleResponse>()
            .ReverseMap();
     
        CreateMap<OrganizationRoles, OrganizationRoleResponse>()
            .ReverseMap();
    }
}