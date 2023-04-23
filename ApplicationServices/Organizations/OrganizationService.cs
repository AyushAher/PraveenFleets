using ApplicationServices.MappingProfile.Organizations;
using AutoMapper;
using DB.Extensions;
using Enums.Account;
using Interfaces.Account;
using Interfaces.Common;
using Interfaces.Organizations;
using Microsoft.Extensions.Logging;
using Shared.Configuration;
using Shared.Requests.Organization;
using Shared.Responses.Organization;

namespace ApplicationServices.Organizations;

public class OrganizationService : IOrganizationService
{

    private readonly ILogger<OrganizationService> _logger;
    private readonly IUnitOfWork<Guid> _unitOfWork;
    private readonly IRepositoryAsync<Domain.Organization.Organizations, Guid> _organizationsRepo;
    private readonly ICacheConfiguration<Domain.Organization.Organizations> _cache;
    private readonly IAddressService _addressService;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly IOrganizationRolesService _organizationRolesService;
    private readonly IOrganizationUserService _organizationUserService;
    private readonly IOrganizationEmployeeService _organizationEmployeeService;
    private readonly ICurrentUserService _currentUserService;

    private const string AdminRoleName = "Admin";

    public OrganizationService(
        IMapper mapper,
        ILogger<OrganizationService> logger,
        IUnitOfWork<Guid> unitOfWork,
        ICacheConfiguration<Domain.Organization.Organizations> cache,
        IAddressService addressService,
        IUserService userService,
        IOrganizationRolesService organizationRolesService,
        IOrganizationUserService organizationUserService,
        IOrganizationEmployeeService organizationEmployeeService,
        ICurrentUserService currentUserService
    )
    {
        _mapper = mapper;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _organizationsRepo = unitOfWork.Repository<Domain.Organization.Organizations>();
        _cache = cache;
        _addressService = addressService;
        _userService = userService;
        _organizationRolesService = organizationRolesService;
        _organizationUserService = organizationUserService;
        _organizationEmployeeService = organizationEmployeeService;
        _currentUserService = currentUserService;
    }


    public async Task<ApiResponse<OrganizationResponse>> RegisterOrganization(
        RegisterOrganizationRequest registerOrganizationRequest)
    {
        try
        {
            registerOrganizationRequest.Id = Guid.NewGuid();
            registerOrganizationRequest.AddressRequest.ParentId = registerOrganizationRequest.Id;
            registerOrganizationRequest.AdminDetailsRequest.ParentEntityId = registerOrganizationRequest.Id;
            registerOrganizationRequest.AdminDetailsRequest.UserType = UserType.Organization;
            registerOrganizationRequest.AdminDetailsRequest.Role = AdminRoleName;

            _ = await _unitOfWork.StartTransaction();


            var addressRegisterRequest =
                await _addressService.CreateAddress(registerOrganizationRequest.AddressRequest, true);

            if (addressRegisterRequest.Failed)
            {
                await _unitOfWork.Rollback();
                return await ApiResponse<OrganizationResponse>.FailAsync(addressRegisterRequest.Messages, _logger);
            }

            var adminRegisterRequest =
                await _userService.RegisterUserAsync(registerOrganizationRequest.AdminDetailsRequest);

            if (adminRegisterRequest.Failed)
            {
                return await ApiResponse<OrganizationResponse>.FailAsync(addressRegisterRequest.Messages, _logger);
            }

            registerOrganizationRequest.AddressRequest.ParentId = adminRegisterRequest.Data.Id;

            var organizationMappedObj = _mapper.Map<Domain.Organization.Organizations>(registerOrganizationRequest);
            organizationMappedObj.AddressId = addressRegisterRequest.Data.Id;
            organizationMappedObj.AdminId = adminRegisterRequest.Data.Id;


            var organizationValidators = new OrganizationValidators();
            var validationRes = await organizationValidators.ValidateAsync(organizationMappedObj);

            if (!validationRes.IsValid)
            {
                return await ApiResponse<OrganizationResponse>.FailAsync(validationRes.Errors, _logger);
            }


            // Add record
            _ = await _organizationsRepo.AddAsync(organizationMappedObj);
            var response = await _unitOfWork.Save(CancellationToken.None);

            // Return if failed
            if (response <= 0)
            {
                await _unitOfWork.Rollback();
                return await ApiResponse<OrganizationResponse>.FailAsync(
                    "Failed To Save Organization. Please try again later!",
                _logger);
            }

            // Commit transaction
            await _unitOfWork.Commit();

            var orgRoleRequestObj = new CreateOrganizationRolesRequest
            {
                RoleName = AdminRoleName,
                User = adminRegisterRequest.Data,
                OrganizationId = adminRegisterRequest.Data.ParentEntityId
            };


            _ = await _organizationRolesService.UpSertUserRole(orgRoleRequestObj);

            var orgUserReq = new RegisterOrganizationUserRequest
            {
                OrganizationId = organizationMappedObj.Id,
                UserId = organizationMappedObj.AdminId,
            };

            _ = await _organizationUserService.AddUserToOrganization(orgUserReq);

            var employeeRequest =
                _mapper.Map<OrganizationEmployeeRequest>(adminRegisterRequest.Data);

            employeeRequest.Gender = registerOrganizationRequest.AdminDetailsRequest.Gender;
            employeeRequest.Salutation = registerOrganizationRequest.AdminDetailsRequest.Salutation;
            employeeRequest.Address = registerOrganizationRequest.AdminDetailsRequest.Address;
            employeeRequest.ParentEntityId = organizationMappedObj.Id;
            employeeRequest.UserId = adminRegisterRequest.Data.Id;
            employeeRequest.PhoneNumber = adminRegisterRequest.Data.PhoneNumber;
            employeeRequest.WeeklyOffs = registerOrganizationRequest.AdminDetailsRequest.WeeklyOffs;

            // Add the new record in cache
            _cache.SetInCacheMemoryAsync(organizationMappedObj);

            var employee = await _organizationEmployeeService.SaveOrganizationEmployee(employeeRequest);

            if (employee.Failed)
            {
                return await ApiResponse<OrganizationResponse>.FailAsync(employee.Messages, _logger);
            }


            // Map post to response obj and return data
            var responseObj = _mapper.Map<OrganizationResponse>(organizationMappedObj);

            return await ApiResponse<OrganizationResponse>.SuccessAsync(responseObj);
        }
        catch (Exception ex)
        {
            return await ApiResponse<OrganizationResponse>.FatalAsync(ex, _logger);
        }
    }

    public async Task<ApiResponse<OrganizationResponse>> GetUserOrganizationDetails()
    {
        try
        {
            if (_currentUserService.UserType != UserType.Organization ||
                _currentUserService.ParentEntityId == Guid.Empty)
            {
                return await ApiResponse<OrganizationResponse>.FailAsync("The User does not belong in any Organization",
                    _logger);
            }


            var organizationCacheObject =
                await _cache.GetFromCacheMemoryByIdAsync(new() { Id = _currentUserService.ParentEntityId });
            if (organizationCacheObject != null)
            {
                var cacheResponseObj = _mapper.Map<OrganizationResponse>(organizationCacheObject);
                return await ApiResponse<OrganizationResponse>.SuccessAsync(cacheResponseObj);
            }

            var organizationObject = await _organizationsRepo.GetByIdAsync(_currentUserService.ParentEntityId);
            var responseObj = _mapper.Map<OrganizationResponse>(organizationObject);
            return await ApiResponse<OrganizationResponse>.SuccessAsync(responseObj);
        }
        catch (Exception e)
        {
            return await ApiResponse<OrganizationResponse>.FatalAsync(e, _logger);
        }
    }

}