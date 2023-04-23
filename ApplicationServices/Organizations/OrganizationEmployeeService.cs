using ApplicationServices.MappingProfile.Organizations;
using AutoMapper;
using DB.Extensions;
using Domain.Organization;
using Enums.Account;
using Interfaces.Account;
using Interfaces.Common;
using Interfaces.Organizations;
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
    private readonly IAddressService _addressService;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly IOrganizationUserService _organizationUserService;

    public OrganizationEmployeeService(
        IMapper mapper,
        ILogger<OrganizationService> logger,
        IUnitOfWork<Guid> unitOfWork,
        ICacheConfiguration<OrganizationEmployee> cache,
        IAddressService addressService,
        IUserService userService,
        IOrganizationUserService organizationUserService
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
    }

    public async Task<ApiResponse<OrganizationEmployeeResponse>> SaveOrganizationEmployee(OrganizationEmployeeRequest request)
    {
        try
        {
            var mappedRequestObj = _mapper.Map<OrganizationEmployee>(request);
            var userMappedRequest = _mapper.Map<RegisterRequest>(request);

            _ = await _unitOfWork.StartTransaction();
            userMappedRequest.UserType = UserType.Organization;
            userMappedRequest.ParentEntityId = request.OrganizationId;

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
            request.AddressRequest.ParentId = employeeUser.Data.Id;

            var addressRequest = await _addressService.CreateAddress(request.AddressRequest, true);
            if (addressRequest.Failed)
            {
                await _unitOfWork.Rollback();
                return await ApiResponse<OrganizationEmployeeResponse>.FailAsync(employeeUser.Messages, _logger);
            }

            mappedRequestObj.AddressId = addressRequest.Data.Id;
            mappedRequestObj.WeeklyOff =
                string.Join(",", request.WeeklyOff.Select(x => x.ToDescriptionString()).ToList());

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
                OrganizationId = request.OrganizationId,
                UserId = employeeUser.Data.Id,
            };
            
            _ = await _organizationUserService.AddUserToOrganization(orgUserReq);
            
            var responseObj = _mapper.Map<OrganizationEmployeeResponse>(mappedRequestObj);
            responseObj.Address = addressRequest.Data;
            responseObj.User = employeeUser.Data;
            

            return await ApiResponse<OrganizationEmployeeResponse>.SuccessAsync();
        }
        catch (Exception e)
        {
            return await ApiResponse<OrganizationEmployeeResponse>.FatalAsync(e, _logger);
        }

    }

}