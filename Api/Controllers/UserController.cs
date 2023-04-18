using Interfaces.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Configuration;
using Shared.Requests.Account;
using Shared.Responses.Account;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{

    private readonly IUserService _accountManager;

    public UserController(IUserService accountManager)
    {
        _accountManager = accountManager;
    }

    //POST: api/identity/account/register
    [HttpPost("Register")]
    [AllowAnonymous]
    public async Task<ApiResponse<UserResponse>> Register(RegisterRequest request)
        => await _accountManager.RegisterUserAsync(request);

    //POST: api/identity/account/register
    [HttpGet("Get/{id}")]
    [AllowAnonymous]
    public async Task<ApiResponse<UserResponse>> Get(string id)
    {
        var Id = Guid.Parse(id);
        return await _accountManager.GetAsync(Id);
    }


    // POST: api/identity/account/regenemailcode
    [HttpPost("RegenEMailCode")]
    [AllowAnonymous]
    public async Task<BaseApiResponse> RegenEMailCode(string request)
        => await _accountManager.RegenEmailConfirmCode(request);


    // POST: api/identity/account/confirmemail
    [HttpPost("ConfirmEmail")]
    [AllowAnonymous]
    public async Task<BaseApiResponse> ConfirmEmail(ConfirmEmailRequest request)
        => await _accountManager.ConfirmEmailAsync(request);


    // POST: api/identity/account/login
    [HttpPost("Authenticate")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<BaseApiResponse> Login(LoginRequest request)
        => await _accountManager.LoginAsync(request);

    
    // POST: api/identity/account/resetpassword
    [HttpPost("ResetPassword")]
    [AllowAnonymous]
    public async Task<BaseApiResponse> ResetPassword(ResetPasswordRequest request)
        => await _accountManager.ResetPasswordAsync(request);

}