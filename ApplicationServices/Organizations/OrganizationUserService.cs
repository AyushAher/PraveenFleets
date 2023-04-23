using ApplicationServices.MappingProfile.Organizations;
using AutoMapper;
using DB.Extensions;
using Domain.Organization;
using Interfaces.Organizations;
using Microsoft.Extensions.Logging;
using Shared.Configuration;
using Shared.Requests.Organization;

namespace ApplicationServices.Organizations;

public class OrganizationUserService: IOrganizationUserService
{
    private readonly ILogger<OrganizationUserService> _logger;
    private readonly IUnitOfWork<Guid> _unitOfWork;
    private readonly IRepositoryAsync<OrganizationUsers, Guid> _orgUsersRepo;
    private readonly IMapper _mapper;
    private readonly ICacheConfiguration<OrganizationUsers> _cache;


    public OrganizationUserService(
        IMapper mapper, 
        ILogger<OrganizationUserService> logger,
        IUnitOfWork<Guid> unitOfWork,
        ICacheConfiguration<OrganizationUsers> cache)
    {

        _mapper = mapper;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _orgUsersRepo = unitOfWork.Repository<OrganizationUsers>();
        _cache = cache;
    }

    public async Task<BaseApiResponse> AddUserToOrganization(RegisterOrganizationUserRequest request)
    {
        try
        {
            
            var doesExists = _orgUsersRepo.Entities.Any(x =>
                x.UserId == request.UserId && x.OrganizationId == request.OrganizationId);

            if (doesExists)
            {
                return await BaseApiResponse.FailAsync("The user already exists in the organization.", _logger);
            }

            var mappedObj = _mapper.Map<OrganizationUsers>(request);
            mappedObj.Id = new Guid();
            
            var validatorObj = new OrganizationUserValidators();
            var validationResult = await validatorObj.ValidateAsync(mappedObj);
            if (!validationResult.IsValid)
                return await BaseApiResponse.FailAsync(validationResult.ToString(), _logger);
            

            _ = await _unitOfWork.StartTransaction();

            await _orgUsersRepo.AddAsync(mappedObj);
            var response = await _unitOfWork.Save(CancellationToken.None);

            // Return if failed
            if (response <= 0)
            {
                await _unitOfWork.Rollback();
                return await BaseApiResponse.FailAsync("Failed To Save Organization. Please try again later!",
                    _logger);
            }

            // Commit transaction
            await _unitOfWork.Commit();

            // Add the new record in cache
            _cache.SetInCacheMemoryAsync(mappedObj);

            return await BaseApiResponse.SuccessAsync();
        }
        catch (Exception e)
        {
            return await BaseApiResponse.FatalAsync(e, _logger);
        }
    }

}