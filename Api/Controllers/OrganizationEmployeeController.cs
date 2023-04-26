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
public class OrganizationEmployeeController : ControllerBase
{
    private readonly IOrganizationEmployeeService _organizationEmployeeService;

    public OrganizationEmployeeController(IOrganizationEmployeeService organizationEmployeeService)
    {
        _organizationEmployeeService = organizationEmployeeService;
    }

    [HttpPost("RegisterEmployee")]
    public async Task<ApiResponse<OrganizationEmployeeResponse>> RegisterEmployee(OrganizationEmployeeRequest request)
        => await _organizationEmployeeService.SaveOrganizationEmployee(request);


    [HttpGet("GetAllOrganizationEmployees")]
    public async Task<ApiResponse<List<OrganizationEmployeeResponse>>> GetAllOrganizationEmployees()
        => await _organizationEmployeeService.GetAllEmpolyees();

}