using ApplicationServices.Repository;
using AutoMapper;
using Domain.Account;
using Interfaces;
using Interfaces.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Shared.Configuration;
using Shared.Requests.Account;
using Shared.Responses.Account;
using Shared.Responses.Organization;

namespace ApplicationServices.Account;

public class RoleClaimService : IRoleClaimService, IService
{
    private readonly ILogger _logger;
    private readonly IRoleClaimsRepository _roleClaimRepo;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public RoleClaimService(
        ILogger<RoleClaimService> logger,
        IRoleClaimsRepository roleClaimsRepository,
        IMapper mapper,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _logger = logger;
        _roleClaimRepo = roleClaimsRepository;
        _mapper = mapper;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<int> GetCountAsync() => await _roleClaimRepo.CountAsync();

    public async Task<ApiResponse<RoleClaimResponse>> GetByIdAsync(int id)
    {
        var data = await _roleClaimRepo.GetByIdAsync(id);
        var mappedObj = _mapper.Map<RoleClaimResponse>(data);
        return await ApiResponse<RoleClaimResponse>.SuccessAsync(mappedObj);
    }

    public async Task<ApiResponse<List<ApplicationRole>>> GetRoleByUserId(Guid userId)
    {
        try
        {
            var roleList = new List<ApplicationRole>();
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var rolesAsync = await _userManager.GetRolesAsync(user);

            if (rolesAsync != null)
            {
                return await ApiResponse<List<ApplicationRole>>.FailAsync();
            }

            foreach (var role in rolesAsync)
                roleList.Add(await _roleManager.FindByNameAsync(role));

            return await ApiResponse<List<ApplicationRole>>.SuccessAsync(roleList);
        }
        catch (Exception e)
        {
            return await ApiResponse<List<ApplicationRole>>.FatalAsync(e, _logger);
        }
    }

    public async Task<ApiResponse<List<RoleClaimResponse>>> GetAllByRoleIdAsync(Guid roleId)
    {
        var data = await _roleClaimRepo.GetAllByRoleIdAsync(roleId);
        var mappedObj = _mapper.Map<List<RoleClaimResponse>>(data);
        return await ApiResponse<List<RoleClaimResponse>>.SuccessAsync(mappedObj);
    }

    public async Task<ApiResponse<List<RoleClaimResponse>>> GetAllAsync()
    {
        var data = await _roleClaimRepo.GetAllAsync();
        var mappedObj = _mapper.Map<List<RoleClaimResponse>>(data);
        return await ApiResponse<List<RoleClaimResponse>>.SuccessAsync(mappedObj);
    }

    public async Task<BaseApiResponse> SaveAsync(RoleClaimRequest request)
    {
        if (request.RoleId == Guid.Empty)
            return await ApiResponse<string>.FailAsync("Role is required.", _logger);

        if (request.Id == 0)
        {
            if (await _roleClaimRepo.CheckRoleCalimExistsAsync(request.RoleId, request.Type, request.Value))
                return await BaseApiResponse.FailAsync("Similar Role Claim already exists.", _logger);

            _ = await _roleClaimRepo.AddAsync(_mapper.Map<ApplicationRoleClaim>(request));
            return await BaseApiResponse.SuccessAsync($"Role Claim {request.Value} created.");
        }

        var existingRoleClaim = await _roleClaimRepo.GetByIdAsync(request.Id);
        existingRoleClaim.ClaimType = request.Type;
        existingRoleClaim.ClaimValue = request.Value;
        existingRoleClaim.Group = request.Group;
        existingRoleClaim.Description = request.Description;
        existingRoleClaim.RoleId = request.RoleId;
        await _roleClaimRepo.UpdateAsync(existingRoleClaim);

        return await BaseApiResponse.SuccessAsync(
            $"Role Claim {request.Value} for Role {existingRoleClaim.Role.Name} updated.");
    }

    public async Task<BaseApiResponse> DeleteAsync(int id)
    {
        var existingRoleClaim = await _roleClaimRepo.GetByIdAsync(id);

        return await _roleClaimRepo.DeleteAsync(id)
            ? await BaseApiResponse.SuccessAsync(
                $"Role Claim {existingRoleClaim.ClaimValue} for {existingRoleClaim.Role.Name} Role deleted.")
            : await BaseApiResponse.FailAsync("Role Claim could not be updated!", _logger);
    }
}
