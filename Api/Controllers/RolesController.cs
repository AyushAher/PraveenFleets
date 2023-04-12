using Interfaces.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Configuration;
using Shared.Requests.Account;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize]
public class RolesController : ControllerBase
{

    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
        => _roleService = roleService;

    [HttpPost("CreateNewRole")]
    public async Task<BaseApiResponse> CreateNewRole(RoleRequest request)
        => await _roleService.SaveAsync(request);


}