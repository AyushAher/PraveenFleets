using Domain.Account;
using Shared.Configuration;
using Shared.Requests.Account;
using Shared.Responses.Account;

namespace Interfaces.Account;

public interface IUserService : IService
{
    Task<ApiResponse<UserResponse>> RegisterUserAsync(RegisterRequest request);

    Task<ApiResponse<TokenResponse>> LoginAsync(LoginRequest request);

    Task<BaseApiResponse> RegenEmailConfirmCode(string emailAddress);

    Task<BaseApiResponse> ConfirmEmailAsync(ConfirmEmailRequest request);

    Task<BaseApiResponse> ResetPasswordAsync(ResetPasswordRequest request);

    Task<BaseApiResponse> UpdatePasswordAsync(UpdatePasswordRequest request);

    Task<BaseApiResponse> UpdateUser(UpdateProfileRequest request);

    Task<BaseApiResponse> RemoveAccount(LoginRequest request);

    Task<BaseApiResponse> RegenWelcomeEMail(string emailAddress);

    Task<ApiResponse<UserResponse>> GetAsync(Guid userId);

    Task<ApiResponse<List<UserResponse>>> GetAllAsync();

    Task<BaseApiResponse> UpdateRolesAsync(UpdateUserRolesRequest request);

    Task<ApiResponse<bool>> RequestEMailAddressChange(UpdateEmailRequest request);

    Task<ApiResponse<bool>> ConfirmEMailAddressChange(UpdateEmailRequest request);

    Task<ApiResponse<bool>> UserExists(string email);
}

