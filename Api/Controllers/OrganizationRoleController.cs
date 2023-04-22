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
public class OrganizationRoleController : ControllerBase
{
    private readonly IOrganizationRolesService _organizationRolesService;

    public OrganizationRoleController(IOrganizationRolesService organizationRolesService)
        => _organizationRolesService = organizationRolesService;
    

    [HttpGet("GetAllOrganizationRoles/{orgId}")]
    public async Task<ApiResponse<List<OrganizationRoleResponse>>> GetAllOrganizationRoles(string orgId)
    {
        var orgIdParsed = Guid.Parse(orgId);
        return await _organizationRolesService.GetOrgRoles(orgIdParsed);
    }

    [HttpPost("CreateOrganizationRole")]
    public async Task<ApiResponse<bool>> CreateOrganizationRole(CreateOrganizationRolesRequest request)
        => await _organizationRolesService.UpSertUserRole(request);

}