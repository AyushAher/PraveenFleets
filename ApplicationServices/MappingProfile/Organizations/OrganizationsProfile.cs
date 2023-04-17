using AutoMapper;
using Shared.Requests.Organization;
using Shared.Responses.Organization;

namespace ApplicationServices.MappingProfile.Organizations;

public class OrganizationsProfile : Profile
{
    public OrganizationsProfile()
    {
        CreateMap<RegisterOrganization, OrganizationResponse>()
            .ReverseMap();
        
        CreateMap<RegisterOrganization, Domain.Organization.Organizations>()
            .ReverseMap();
     
        CreateMap<Domain.Organization.Organizations, OrganizationResponse>()
            .ReverseMap();
    }
}