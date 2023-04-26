using AutoMapper;
using Domain.Organization;
using Shared.Requests.Account;
using Shared.Requests.Organization;
using Shared.Responses.Account;
using Shared.Responses.Organization;

namespace ApplicationServices.MappingProfile.Organizations;

public class OrganizationEmployeeProfile : Profile
{
    public OrganizationEmployeeProfile()
    {
        CreateMap<OrganizationEmployee, OrganizationEmployeeResponse>()
            .ForMember(x => x.WeeklyOff, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(x => x.WeeklyOff, opt => opt.Ignore());
        
        CreateMap<OrganizationEmployeeRequest, OrganizationEmployeeResponse>()
            .ReverseMap();
        
        CreateMap<OrganizationEmployeeRequest, OrganizationEmployee>()
            .ReverseMap();

        CreateMap<OrganizationEmployeeRequest, RegisterRequest>()
            .ReverseMap();

        CreateMap<OrganizationEmployeeRequest, UserResponse>()
            .ReverseMap();

        CreateMap<OrganizationEmployeeResponse, Vw_OrganizationEmployee>()
            .ReverseMap();
    }
}