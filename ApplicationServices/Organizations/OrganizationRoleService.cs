using AutoMapper;
using DB.Extensions;
using Domain.Account;
using Domain.Organization;
using Interfaces.Organizations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Configuration;
using Shared.Responses.Account;
using Utility.Email;

namespace ApplicationServices.Organizations;

public class OrganizationRoleService : IOrganizationRolesService
{
    private readonly IMapper _mapper;
    private readonly ILogger<OrganizationRoleService> _logger;
    private readonly ICacheConfiguration<ApplicationUser> _cache;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IUnitOfWork<Guid> _unitOfWork;

    public OrganizationRoleService(
        ICacheConfiguration<ApplicationUser> cache,
        IMapper mapper,
        ILogger<OrganizationRoleService> logger,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IUnitOfWork<Guid> unitOfWork
    )
    {
        _roleManager = roleManager;
        _mapper = mapper;
        _logger = logger;
        _cache = cache;
        _userManager = userManager;
        _roleManager = roleManager;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<ApiResponse<bool>> UpSertUserRole(string role, UserResponse userResponse)
    {
        try
        {
            _ = await _unitOfWork.StartTransaction();
            var user = _mapper.Map<ApplicationUser>(userResponse);
            // Check if role exists in org. as well as in roles
            if (!await _roleManager.RoleExistsAsync(role))
            {
                var applicationRole = new ApplicationRole()
                {
                    Id = Guid.NewGuid(),
                    Name = role,
                    NormalizedName = _roleManager.NormalizeKey(role),
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

            var roles = await _roleManager.FindByNameAsync(role);

            var orgRole = new OrganizationRoles
            {
                Id = Guid.NewGuid(),
                OrganizationId = user.ParentEntityId,
                RoleId = roles.Id
            };

            await _unitOfWork.Repository<OrganizationRoles>().AddAsync(orgRole);
            var response = await _unitOfWork.Save(CancellationToken.None);

            // Return if failed
            if (response <= 0)
            {
                await _unitOfWork.Rollback();
                return await ApiResponse<bool>.FailAsync(
                    "Failed To Save Organization Role. Please try again later!",
                    _logger);
            }

            // Assign role to user
            if (!(await _userManager.AddToRoleAsync(user, role)).Succeeded)
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

}