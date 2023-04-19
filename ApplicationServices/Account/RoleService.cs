using ApplicationServices.Repository;
using AutoMapper;
using Domain.Account;
using Enums.Account;
using Interfaces;
using Interfaces.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Configuration;
using Shared.Requests.Account;
using Shared.Responses.Account;
using Utility.Extensions;

namespace ApplicationServices.Account;

public class RoleService : IRoleService, IService
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRoleClaimService _roleClaimService;

    public RoleService(
        ILogger<RoleService> logger,
        IMapper mapper,
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager,
        IRoleClaimService roleClaimService)
    {
        _logger = logger;
        _mapper = mapper;
        _roleManager = roleManager;
        _userManager = userManager;
        _roleClaimService = roleClaimService;
    }

    public async Task<int> GetCountAsync() => await _roleManager.Roles.CountAsync();

    public async Task<ApiResponse<RoleResponse>> GetByIdAsync(Guid id)
    {
        var data = await _roleManager.Roles.SingleOrDefaultAsync(x => x.Id == id);
        var mappedObj = _mapper.Map<RoleResponse>(data);
        return await ApiResponse<RoleResponse>.SuccessAsync(mappedObj);
    }
    
    public async Task<ApiResponse<RoleResponse>> GetByName(string roleName)
    {
        var data = await _roleManager.FindByNameAsync(roleName);
        var mappedObj = _mapper.Map<RoleResponse>(data);
        return await ApiResponse<RoleResponse>.SuccessAsync(mappedObj);
    }

    public async Task<ApiResponse<List<RoleResponse>>> GetAllAsync()
    {
        var data = await _roleManager.Roles.ToListAsync();
        var mappedObj = _mapper.Map<List<RoleResponse>>(data);
        return await ApiResponse<List<RoleResponse>>.SuccessAsync(mappedObj);
    }

    public async Task<BaseApiResponse> SaveAsync(RoleRequest request)
    {
        if (request.Id == Guid.Empty)
        {
            if (await _roleManager.RoleExistsAsync(request.Name))
                return await BaseApiResponse.FailAsync("Similar Role already exists.", _logger);

            var async = await _roleManager.CreateAsync(new ApplicationRole(request.Name, request.Description));

            return async.Succeeded
                ? await BaseApiResponse.SuccessAsync($"Role {request.Name} Created.")
                : await BaseApiResponse.FailAsync(async.Errors.Select(x => x.Description).ToList(), _logger);
        }

        var existingRole = await _roleManager.FindByIdAsync(request.Id.ToString());
        
        if (existingRole is { Name: "Admin" })
                return await BaseApiResponse.FailAsync($"Not allowed to modify {existingRole.Name} Role.", _logger);
      

        existingRole.Name = request.Name;
        existingRole.NormalizedName = request.Name.ToUpper();
        existingRole.Description = request.Description;

        await _roleManager.UpdateAsync(existingRole);

        return await BaseApiResponse.SuccessAsync($"Role {existingRole.Name} Updated.");
    }

    public async Task<BaseApiResponse> DeleteAsync(Guid id)
    {
        var existingRole = await _roleManager.FindByIdAsync(id.ToString());
        if (existingRole.Name == "Admin")
            return await BaseApiResponse.SuccessAsync($"Not allowed to delete {existingRole.Name} Role.");
        var roleIsNotUsed = true;

        foreach (var user in await _userManager.Users.ToListAsync())
        {
            if (!await _userManager.IsInRoleAsync(user, existingRole.Name)) continue;
            roleIsNotUsed = false;
            break;
        }

        if (!roleIsNotUsed)
            return await BaseApiResponse.SuccessAsync(
                $"Not allowed to delete {existingRole.Name} Role as it is being used.");

        await _roleManager.DeleteAsync(existingRole);

        return await BaseApiResponse.SuccessAsync($"Role {existingRole.Name} Deleted.");
    }

    public async Task<ApiResponse<PermissionResponse>> GetAllPermissionsAsync(Guid roleId)
    {
        var model = new PermissionResponse();
        var allPermissions = GetAllPermissions();
        var byIdAsync = await _roleManager.FindByIdAsync(roleId.ToString());
        if (byIdAsync != null)
        {
            model.RoleId = byIdAsync.Id;
            model.RoleName = byIdAsync.Name;
            var allByRoleIdAsync =
                await _roleClaimService.GetAllByRoleIdAsync(byIdAsync.Id);

            if (allByRoleIdAsync.Succeeded)
            {
                var data = allByRoleIdAsync.Data;
                var list = allPermissions.Select(a => a.Value).ToList()
                    .Intersect(data.Select(a => a.Value).ToList())
                    .ToList();

                foreach (var roleClaimResponse1 in allPermissions)
                {
                    var permission = roleClaimResponse1;
                    if (!list.Any(a => a == permission.Value)) continue;

                    permission.Selected = true;

                    var roleClaimResponse2 = data.SingleOrDefault(a => a.Value == permission.Value);

                    if (roleClaimResponse2?.Description != null)
                        permission.Description = roleClaimResponse2.Description;

                    if (roleClaimResponse2?.Group != null)
                        permission.Group = roleClaimResponse2.Group;
                }
            }
            else
            {
                model.RoleClaims = new List<RoleClaimResponse>();
                return await ApiResponse<PermissionResponse>.FailAsync(allByRoleIdAsync.Messages, _logger);
            }
        }

        model.RoleClaims = allPermissions;
        return await ApiResponse<PermissionResponse>.SuccessAsync(model);
    }

    public async Task<BaseApiResponse> UpdatePermissionsAsync(PermissionRequest request)
    {
        try
        {
            var errors = new List<string>();
            var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());
            
            var selectedClaims = request.RoleClaims
                .Where(a => a.Selected).ToList();

            foreach (var claim in await _roleManager.GetClaimsAsync(role))
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }

            foreach (var roleClaimRequest in selectedClaims)
            {
                var identityResult =
                    await _roleManager.AddPermissionClaim(role, roleClaimRequest.Value);
                if (!identityResult.Succeeded)
                {
                    errors.AddRange(identityResult.Errors.Select(x => x.Description));
                }
            }

            var addedClaims = await _roleClaimService.GetAllByRoleIdAsync(role.Id);
            if (addedClaims.Succeeded)
            {
                foreach (var roleClaimRequest in selectedClaims)
                {
                    var claim = roleClaimRequest;
                    var roleClaimResponse =
                        addedClaims.Data.SingleOrDefault(x => x.Type == claim.Type && x.Value == claim.Value);
                    if (roleClaimResponse == null) continue;

                    claim.Id = roleClaimResponse.Id;
                    claim.RoleId = roleClaimResponse.RoleId;

                    var apiResponse = await _roleClaimService.SaveAsync(claim);
                    if (!apiResponse.Succeeded) errors.AddRange(apiResponse.Messages);
                }
            }
            else
                errors.AddRange(addedClaims.Messages);

            return errors.Any()
                ? await BaseApiResponse.FailAsync(errors, _logger)
                : await BaseApiResponse.SuccessAsync("Permissions Updated.");
        }
        catch (Exception ex)
        {
            return await BaseApiResponse.FatalAsync(ex, _logger);
        }
    }

    private static List<RoleClaimResponse> GetAllPermissions()
    {
        var allPermissions = new List<RoleClaimResponse>();
        allPermissions.GetAllPermissions();
        return allPermissions;
    }
}
