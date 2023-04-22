using Interfaces.Organizations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Configuration;
using Shared.Requests.Organization;
using Shared.Responses.Organization;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
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
    
    [HttpGet("GetUserOrganizationDetails")]
    public async Task<ApiResponse<OrganizationResponse>> GetUserOrganizationDetails()
        => await _organizationService.GetUserOrganizationDetails();
}