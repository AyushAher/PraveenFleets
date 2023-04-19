using AutoMapper;
using Shared.Requests.Organization;
using Shared.Responses.Organization;

namespace ApplicationServices.MappingProfile.Organizations;

public class OrganizationsProfile : Profile
{
    public OrganizationsProfile()
    {
        CreateMap<RegisterOrganizationRequest, OrganizationResponse>()
            .ReverseMap();
        
        CreateMap<RegisterOrganizationRequest, Domain.Organization.Organizations>()
            .ReverseMap();
     
        CreateMap<Domain.Organization.Organizations, OrganizationResponse>()
            .ReverseMap();
    }
}