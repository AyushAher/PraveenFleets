using System.Text;
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
    private readonly ICacheConfiguration<OrganizationRoles> _cache;
    private readonly ICacheConfiguration<Vw_OrganizationRoles> _orgRoleCache;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IUnitOfWork<Guid> _unitOfWork;
    private readonly IRoleService _roleService;
    private readonly IUserService _userService;
    private readonly IRepositoryAsync<OrganizationRoles, Guid> _organizationRolesRepo;

    public OrganizationRoleService(
        ICacheConfiguration<OrganizationRoles> cache,
        IMapper mapper,
        ILogger<OrganizationRoleService> logger,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IUnitOfWork<Guid> unitOfWork,
        IRoleService roleService,
        IUserService userService,
        ICacheConfiguration<Vw_OrganizationRoles> orgRoleCache)
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
        _userService = userService;
        _orgRoleCache = orgRoleCache;
    }

    public async Task<ApiResponse<bool>> UpSertUserRole(CreateOrganizationRolesRequest request)
    {
        try
        {
           // _ = await _unitOfWork.StartTransaction();
            
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
                   // await _unitOfWork.Rollback();
                    foreach (var error in result.Errors)
                        _logger.LogError("Error Code : {0}; Description : {1}",
                            error.Code, error.Description);
                    return await ApiResponse<bool>.FailAsync("E:" + "Sorry we have a failure. Please contact Support!",
                        _logger);
                }
            }

            var roles = await _roleManager.FindByNameAsync(request.RoleName);
            var objExists = _organizationRolesRepo.Entities.Any(x =>
                                            x.RoleId == roles.Id && 
                                            x.OrganizationId == request.OrganizationId);

            if (!objExists)
            {
                var orgRole = new OrganizationRoles
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = request.OrganizationId,
                    RoleId = roles.Id
                };

                await _organizationRolesRepo.AddAsync(orgRole);
                var response = await _unitOfWork.Save(CancellationToken.None);

                // Return if failed
                if (response <= 0)
                {
                  //  await _unitOfWork.Rollback();
                    return await ApiResponse<bool>.FailAsync(
                        "Failed To Save Organization Role. Please try again later!",
                        _logger);
                }
            }

            if (request.User == null)
            {
                // Commit transaction
                //await _unitOfWork.Commit();
                return await ApiResponse<bool>.SuccessAsync(true);
            }

            var user = await _userManager.FindByNameAsync(request.User.Email);
            
            //TODO Check why Organization User save is called here

            // Assign role to user
            var userRole = await _userManager.AddToRoleAsync(user, request.RoleName);
            if (!userRole.Succeeded)
            {
              //  await _unitOfWork.Rollback();

                _logger.LogError("Unable to create the User Role for User {0} / {1}. Check Error Log!",
                    user.Email, user.FirstName + " " + user.LastName);

                return await ApiResponse<bool>.FailAsync("E:" + "Sorry we have a failure. Please contact Support!",
                    _logger);
            }

            // Commit transaction
            //await _unitOfWork.Commit();

            return await ApiResponse<bool>.SuccessAsync(true);
        }
        catch (Exception ex)
        {
            return await ApiResponse<bool>.FatalAsync(ex, _logger);
        }
    }

    public async Task<ApiResponse<List<OrganizationRoleResponse>>> GetOrgRoles(Guid organizationId)
    {
        try
        {
            if (organizationId == Guid.Empty)
            {
                return await ApiResponse<List<OrganizationRoleResponse>>.FailAsync(
                    "Some Error occurred, while querying for user role.", _logger);
            }

            var getFromCache =
                await _cache.GetAllFromCacheMemoryAsync();

            if (getFromCache is { Count: > 0 } && getFromCache.Count ==
                _organizationRolesRepo.Entities.Count(x => x.OrganizationId == organizationId))
            {
                var cacheResponseMappedObj = _mapper.Map<List<OrganizationRoleResponse>>(getFromCache);
                return await ApiResponse<List<OrganizationRoleResponse>>.SuccessAsync(cacheResponseMappedObj);
            }

            var queryRequest = new GetAllRoleQueryRequest
            {
                CheckOrganization = true,
                ParamStr = organizationId.ToString()
            };

            var lst = await _unitOfWork.Repository<Vw_OrganizationRoles>().GetAllAsync(GetAllViewQuery(queryRequest));
            lst.ForEach(x => _orgRoleCache.SetInCacheMemoryAsync(x));

            var responseMappedObj = _mapper.Map<List<OrganizationRoleResponse>>(lst);

            return await ApiResponse<List<OrganizationRoleResponse>>.SuccessAsync(responseMappedObj);
        }
        catch (Exception e)
        {
            return await ApiResponse<List<OrganizationRoleResponse>>.FatalAsync(e, _logger);
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

            var user = await _userService.GetAsync(userId);
            if (user.Failed)
            {
                return await ApiResponse<OrganizationRoleResponse>.FailAsync(user.Messages, _logger);
            }
            var userMapped = _mapper.Map<ApplicationUser>(user);

            var userRoles = await _userManager.GetRolesAsync(userMapped);
            if (userRoles.Count <= 0)
            {
                return await ApiResponse<OrganizationRoleResponse>.FailAsync("No Roles assigned for this user", _logger);
            }

            var fodRole = userRoles.FirstOrDefault();
            if (fodRole == null)
            {
                return await ApiResponse<OrganizationRoleResponse>.FailAsync("No Roles assigned for this user", _logger);
            }


            var userRoleName = await _roleService.GetByName(fodRole);



            return await ApiResponse<OrganizationRoleResponse>.SuccessAsync();
        }
        catch (Exception e)
        {
            return await ApiResponse<OrganizationRoleResponse>.FatalAsync(e, _logger);
        }
    }

    private string GetAllViewQuery(GetAllRoleQueryRequest request)
    {
        var queryStr = new StringBuilder("(select * from `Vw_OrganizationRoles` where (`Vw_OrganizationRoles`.`DeletedBy` is null)");

        if (request.CheckRole)
            queryStr.Append($"and (`Vw_OrganizationRoles`.`RoleId`=\"{request.ParamStr}\" )");

        else if (request.CheckOrganization)
            queryStr.Append($"and (`Vw_OrganizationRoles`.`OrganizationId`=\"{request.ParamStr}\" )");

        queryStr.Append(");");
        return queryStr.ToString();
    }
}