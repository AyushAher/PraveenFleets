using AutoMapper;
using Domain.Organization;
using Shared.Requests.Organization;
using Shared.Responses.Organization;

namespace ApplicationServices.MappingProfile.Organizations;

public class OrganizationUserProfile:Profile
{
    public OrganizationUserProfile()
    {
        CreateMap<RegisterOrganizationUserRequest, OrganizationUsers>()
            .ReverseMap();
        
        CreateMap<OrganizationUserResponse, OrganizationUsers>()
            .ReverseMap();
        
        CreateMap<RegisterOrganizationUserRequest, OrganizationUserResponse>()
            .ReverseMap();

    }
}