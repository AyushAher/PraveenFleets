using Interfaces.Account;
using Interfaces.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Configuration;
using Shared.Responses.Common;

namespace Api.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class AddressController : ControllerBase
{
    private readonly IAddressService _addressService;
    private readonly ICurrentUserService _currentUserService;


    public AddressController(
        IAddressService addressService,
        ICurrentUserService currentUserService
    )
    {
        _addressService = addressService;
        _currentUserService = currentUserService;
    }

    [HttpGet("GetByParentId/{parentIdStr}")]
    public async Task<ApiResponse<AddressResponse>> GetAddressByParentId(string parentIdStr)
    {
        var parentId = Guid.Parse(parentIdStr);
        return await _addressService.GetAddressByParentId(parentId);
    }

    [HttpGet("GetAddressByCurrentUserId")]
    public async Task<ApiResponse<AddressResponse>> GetAddressByParentId()
        => await _addressService.GetAddressByParentId(_currentUserService.UserId);
    
    [HttpGet("GetAddressByCurrentUserParentId")]
    public async Task<ApiResponse<AddressResponse>> GetAddressByCurrentUserParentId()
        => await _addressService.GetAddressByParentId(_currentUserService.ParentEntityId);


}