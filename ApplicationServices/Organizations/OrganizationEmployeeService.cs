using ApplicationServices.Account;
using ApplicationServices.MappingProfile.Organizations;
using AutoMapper;
using DB.Extensions;
using Domain.Organization;
using Enums.Account;
using Interfaces.Account;
using Interfaces.Common;
using Interfaces.Organizations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Configuration;
using Shared.Requests.Account;
using Shared.Requests.Organization;
using Shared.Responses.Account;
using Shared.Responses.Organization;
using Utility.Extensions;

namespace ApplicationServices.Organizations;

public class OrganizationEmployeeService : IOrganizationEmployeeService
{

    private readonly ILogger<OrganizationService> _logger;
    private readonly IUnitOfWork<Guid> _unitOfWork;
    private readonly IRepositoryAsync<OrganizationEmployee, Guid> _organizationEmployeeRepo;
    private readonly ICacheConfiguration<OrganizationEmployee> _cache;
    private readonly ICacheConfiguration<Vw_OrganizationEmployee> _vwOrgEmployeeCache;
    private readonly IAddressService _addressService;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly IOrganizationUserService _organizationUserService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IOrganizationRolesService _organizationRolesService;
    private readonly IRoleService _roleService;

    public OrganizationEmployeeService(
        IMapper mapper,
        ILogger<OrganizationService> logger,
        IUnitOfWork<Guid> unitOfWork,
        ICacheConfiguration<OrganizationEmployee> cache,
        ICacheConfiguration<Vw_OrganizationEmployee> vwOrgEmployeeCache,
        IAddressService addressService,
        IUserService userService,
        IOrganizationUserService organizationUserService,
        ICurrentUserService currentUserService,
        IOrganizationRolesService organizationRolesService,
        IRoleService roleService
    )
    {
        _mapper = mapper;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _organizationEmployeeRepo = unitOfWork.Repository<OrganizationEmployee>();
        _cache = cache;
        _addressService = addressService;
        _userService = userService;
        _organizationUserService = organizationUserService;
        _currentUserService = currentUserService;
        _vwOrgEmployeeCache = vwOrgEmployeeCache;
        _organizationRolesService = organizationRolesService;
        _roleService = roleService;

    }

    public async Task<ApiResponse<OrganizationEmployeeResponse>> SaveOrganizationEmployee(OrganizationEmployeeRequest request)
    {
        try
        {
            var mappedRequestObj = _mapper.Map<OrganizationEmployee>(request);
            var userMappedRequest = _mapper.Map<RegisterRequest>(request);

            _ = await _unitOfWork.StartTransaction();
            userMappedRequest.UserType = UserType.Organization;

            userMappedRequest.ParentEntityId = request.ParentEntityId == Guid.Empty
                ? _currentUserService.ParentEntityId
                : request.ParentEntityId;

            mappedRequestObj.OrganizationId = request.ParentEntityId == Guid.Empty
                ? _currentUserService.ParentEntityId
                : request.ParentEntityId;

            ApiResponse<UserResponse> employeeUser;

            if (request.UserId == Guid.Empty)
            {
                employeeUser = await _userService.RegisterUserAsync(userMappedRequest);
                if (employeeUser.Failed)
                {
                    await _unitOfWork.Rollback();
                    return await ApiResponse<OrganizationEmployeeResponse>.FailAsync(employeeUser.Messages, _logger);
                }
            }
            else employeeUser = await _userService.GetAsync(request.UserId);


            mappedRequestObj.UserId = employeeUser.Data.Id;
            request.Address.ParentId = employeeUser.Data.Id;

            var addressRequest = await _addressService.CreateAddress(request.Address, true);
            if (addressRequest.Failed)
            {
                await _unitOfWork.Rollback();
                return await ApiResponse<OrganizationEmployeeResponse>.FailAsync(employeeUser.Messages, _logger);
            }

            mappedRequestObj.AddressId = addressRequest.Data.Id;
            mappedRequestObj.WeeklyOff =
                string.Join(",", request.WeeklyOffs.Select(x => x.ToDescriptionString()).ToList());

            var validationObj = new OrganizationEmployeeValidator();
            var result = await validationObj.ValidateAsync(mappedRequestObj);
            if (!result.IsValid)
            {
                return await ApiResponse<OrganizationEmployeeResponse>.FailAsync(result.Errors, _logger);
            }

            mappedRequestObj.Id = new Guid();

            _ = await _organizationEmployeeRepo.AddAsync(mappedRequestObj);

            var response = await _unitOfWork.Save(CancellationToken.None);

            // Return if failed
            if (response <= 0)
            {
                await _unitOfWork.Rollback();
                return await ApiResponse<OrganizationEmployeeResponse>.FailAsync(
                    "Failed To Save Employee. Please try again later!",
                    _logger);
            }

            // Commit transaction
            await _unitOfWork.Commit();

            _cache.SetInCacheMemoryAsync(mappedRequestObj);


            var orgUserReq = new RegisterOrganizationUserRequest
            {
                OrganizationId = userMappedRequest.ParentEntityId,
                UserId = employeeUser.Data.Id,
            };

            var roleName = await _roleService.GetByIdAsync(request.RoleId);

            var userOrg = await _organizationUserService.AddUserToOrganization(orgUserReq);
            if (!roleName.Failed && roleName.Data != null)
            {
                var requestRole = new CreateOrganizationRolesRequest()
                {
                    OrganizationId = userMappedRequest.ParentEntityId,
                    User = employeeUser.Data,
                    RoleName = roleName.Data.Name
                };
                var x = await _organizationRolesService.UpSertUserRole(requestRole);
            }

            return await ApiResponse<OrganizationEmployeeResponse>.SuccessAsync();
        }
        catch (Exception e)
        {
            return await ApiResponse<OrganizationEmployeeResponse>.FatalAsync(e, _logger);
        }

    }


    public async Task<ApiResponse<List<OrganizationEmployeeResponse>>> GetAllEmpolyees()
    {
        try
        {
            // TODO Set user role when employee created
            // TODO get all data that is required
            List<OrganizationEmployeeResponse> lstResponse;

            var cacheObject = await _vwOrgEmployeeCache.GetAllFromCacheMemoryAsync(new());
            if (cacheObject.Count == await _unitOfWork.Repository<Vw_OrganizationEmployee>().GetCount())
            {
                lstResponse = _mapper.Map<List<OrganizationEmployeeResponse>>(cacheObject);
                lstResponse.ForEach(x => x.Address = _addressService.GetAddressByParentId(x.UserId).Result.Data);
                return await ApiResponse<List<OrganizationEmployeeResponse>>.SuccessAsync(lstResponse);
            }

            var dbObject =
                await _unitOfWork.Repository<Vw_OrganizationEmployee>().Entities.Where(x =>
                    x.OrganizationId == _currentUserService.ParentEntityId).ToListAsync();
            
            dbObject.ForEach(x => _vwOrgEmployeeCache.SetInCacheMemoryAsync(x));
            lstResponse = _mapper.Map<List<OrganizationEmployeeResponse>>(dbObject);
            lstResponse.ForEach(x =>
            {
                x.Address = _addressService.GetAddressByParentId(x.UserId).Result.Data;
            });


            return await ApiResponse<List<OrganizationEmployeeResponse>>.SuccessAsync(lstResponse);
        }
        catch (Exception e)
        {
            return await ApiResponse<List<OrganizationEmployeeResponse>>.FatalAsync(e, _logger);
        }
    }
}