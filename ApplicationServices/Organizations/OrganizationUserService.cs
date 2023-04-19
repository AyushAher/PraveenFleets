using ApplicationServices.Account;
using ApplicationServices.Common;
using ApplicationServices.MappingProfile.Organizations;
using AutoMapper;
using DB.Extensions;
using Domain.Organization;
using Interfaces.Account;
using Interfaces.Common;
using Interfaces.Organizations;
using Microsoft.Extensions.Logging;
using Shared.Configuration;
using Shared.Requests.Organization;
using Shared.Responses.Organization;

namespace ApplicationServices.Organizations;

public class OrganizationUserService: IOrganizationUserService
{
    private readonly ILogger<OrganizationUserService> _logger;
    private readonly IUnitOfWork<Guid> _unitOfWork;
    private readonly IRepositoryAsync<OrganizationUsers, Guid> _orgUsersRepo;
    private readonly IMapper _mapper;
    private readonly ICacheConfiguration<OrganizationUsers> _cache;
    private readonly IOrganizationRolesService _organizationRolesService;
    private readonly IRoleClaimService _roleClaimService;


    public OrganizationUserService(
        IMapper mapper, 
        ILogger<OrganizationUserService> logger,
        IUnitOfWork<Guid> unitOfWork,
        ICacheConfiguration<OrganizationUsers> cache,
        IOrganizationRolesService organizationRolesService,
        IRoleClaimService roleClaimService
        )
    {

        _mapper = mapper;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _orgUsersRepo = unitOfWork.Repository<OrganizationUsers>();
        _cache = cache;
        _organizationRolesService = organizationRolesService;
        _roleClaimService = roleClaimService;
    }

    public async void AddUserToOrganization(RegisterOrganizationUserRequest request)
    {
        try
        {
            var mappedObj = _mapper.Map<OrganizationUsers>(request);

            var userRoleListResponse =  await _roleClaimService.GetRoleByUserId(request.UserId);
            if (userRoleListResponse.Failed || userRoleListResponse.Data is not { Count: > 0 })
                return;

            var role = userRoleListResponse.Data.FirstOrDefault();
            if (role == null) return;

            mappedObj.RoleId = role.Id;
            
            var validatorObj = new OrganizationUserValidators();
            var validationResult = await validatorObj.ValidateAsync(mappedObj);
            if (!validationResult.IsValid)
            {
                await BaseApiResponse.FailAsync(validationResult.ToString(), _logger);
                return;
            }
            
            _ = await _unitOfWork.StartTransaction();

            await _orgUsersRepo.AddAsync(mappedObj);
            var response = await _unitOfWork.Save(CancellationToken.None);
            
            // Return if failed
            if (response <= 0)
            {
                await _unitOfWork.Rollback();
                await BaseApiResponse.FailAsync("Failed To Save Organization. Please try again later!",
                    _logger);

                return;
            }

            // Commit transaction
            await _unitOfWork.Commit();

            // Add the new record in cache
            _cache.SetInCacheMemoryAsync(mappedObj);

        }
        catch (Exception e)
        {
            await BaseApiResponse.FatalAsync(e, _logger);
        }
    }

}