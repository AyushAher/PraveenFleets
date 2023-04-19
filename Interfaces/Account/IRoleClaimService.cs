using Domain.Account;
using Shared.Configuration;
using Shared.Requests.Account;
using Shared.Responses.Account;

namespace Interfaces.Account;


public interface IRoleClaimService : IService
{
    Task<int> GetCountAsync();

    Task<ApiResponse<RoleClaimResponse>> GetByIdAsync(int id);
    
    Task<ApiResponse<List<ApplicationRole>>> GetRoleByUserId(Guid userId);
    
    Task<ApiResponse<List<RoleClaimResponse>>> GetAllByRoleIdAsync(Guid roleId);

    Task<ApiResponse<List<RoleClaimResponse>>> GetAllAsync();

    Task<BaseApiResponse> SaveAsync(RoleClaimRequest request);

    Task<BaseApiResponse> DeleteAsync(int id);
}