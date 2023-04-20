using AutoMapper;
using DB.Extensions;
using Domain.Account;
using Domain.Organization;
using Interfaces.Account;
using Interfaces.Organizations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Shared.Configuration;
using Shared.Requests.Organization;
using Shared.Responses.Organization;

namespace ApplicationServices.Organizations;

public class OrganizationRoleService : IOrganizationRolesService
{
    private readonly IMapper _mapper;
    private readonly ILogger<OrganizationRoleService> _logger;
    private readonly ICacheConfiguration<ApplicationUser> _cache;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IUnitOfWork<Guid> _unitOfWork;
    private readonly IRoleService _roleService;
    private readonly IRepositoryAsync<OrganizationRoles, Guid> _organizationRolesRepo;

    public OrganizationRoleService(
        ICacheConfiguration<ApplicationUser> cache,
        IMapper mapper,
        ILogger<OrganizationRoleService> logger,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IUnitOfWork<Guid> unitOfWork,
        IRoleService roleService
    )
    {
        _roleManager = roleManager;
        _mapper = mapper;
        _logger = logger;
        _cache = cache;
        _userManager = userManager;
        _roleManager = roleManager;
        _unitOfWork = unitOfWork;
        _roleService = roleService;
        _organizationRolesRepo = _unitOfWork.Repository<OrganizationRoles>();
    }
    
    public async Task<ApiResponse<bool>> UpSertUserRole(CreateOrganizationRolesRequest request)
    {
        try
        {
            _ = await _unitOfWork.StartTransaction();
            var user = await _userManager.FindByNameAsync(request.User.Email);
            
            // Check if role exists in org. as well as in roles
            if (!await _roleManager.RoleExistsAsync(request.RoleName))
            {
                var applicationRole = new ApplicationRole()
                {
                    Id = Guid.NewGuid(),
                    Name = request.RoleName,
                    NormalizedName = _roleManager.NormalizeKey(request.RoleName),
                };

                var result = await _roleManager.CreateAsync(applicationRole);
                if (!result.Succeeded)
                {
                    await _unitOfWork.Rollback();
                    foreach (var error in result.Errors)
                        _logger.LogError("Error for User {0} - Code : {1}; Description : {2}", user.Email,
                            error.Code, error.Description);
                    return await ApiResponse<bool>.FailAsync("E:" + "Sorry we have a failure. Please contact Support!",
                        _logger);
                }
            }

            var roles = await _roleManager.FindByNameAsync(request.RoleName);
            var objExists = _organizationRolesRepo.Entities.Any(x =>
                                            x.RoleId == roles.Id && 
                                            x.OrganizationId == user.ParentEntityId);

            if (!objExists)
            {
                var orgRole = new OrganizationRoles
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = user.ParentEntityId,
                    RoleId = roles.Id
                };

                await _organizationRolesRepo.AddAsync(orgRole);
                var response = await _unitOfWork.Save(CancellationToken.None);

                // Return if failed
                if (response <= 0)
                {
                    await _unitOfWork.Rollback();
                    return await ApiResponse<bool>.FailAsync(
                        "Failed To Save Organization Role. Please try again later!",
                        _logger);
                }
            }

            // Assign role to user
            if (!(await _userManager.AddToRoleAsync(user, request.RoleName)).Succeeded)
            {
                await _unitOfWork.Rollback();

                _logger.LogError("Unable to create the User Role for User {0} / {1}. Check Error Log!",
                    user.Email, user.FirstName + " " + user.LastName);

                return await ApiResponse<bool>.FailAsync("E:" + "Sorry we have a failure. Please contact Support!",
                    _logger);
            }

            // Commit transaction
            await _unitOfWork.Commit();

            return await ApiResponse<bool>.SuccessAsync(true);
        }
        catch (Exception ex)
        {
            return await ApiResponse<bool>.FatalAsync(ex, _logger);
        }
    }

    public async Task<ApiResponse<OrganizationRoleResponse>> GetOrgRoleByUserId(Guid userId)
    {
        try
        {
            if (userId == Guid.Empty)
            {
                return await ApiResponse<OrganizationRoleResponse>.FailAsync(
                    "Some Error occurred, while querying for user role.", _logger);
            }
            
            
            


            return await ApiResponse<OrganizationRoleResponse>.SuccessAsync();
        }
        catch (Exception e)
        {
            return await ApiResponse<OrganizationRoleResponse>.FatalAsync(e, _logger);
        }
    }
}