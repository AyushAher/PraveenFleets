using Interfaces.Organizations;
using Microsoft.AspNetCore.Mvc;
using Shared.Configuration;
using Shared.Requests.Organization;
using Shared.Responses.Organization;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrganizationController : ControllerBase
{
    private readonly IOrganizationService _organizationService;

    public OrganizationController(IOrganizationService organizationService)
    {
        _organizationService = organizationService;
    }

    [HttpPost("Register")]
    public async Task<ApiResponse<OrganizationResponse>> RegisterOrganization(
        RegisterOrganizationRequest registerOrganizationRequest)
        => await _organizationService.RegisterOrganization(registerOrganizationRequest);
}