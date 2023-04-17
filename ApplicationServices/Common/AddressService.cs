using ApplicationServices.MappingProfile.Common;
using AutoMapper;
using DB.Extensions;
using Domain.Common;
using Interfaces.Common;
using Microsoft.Extensions.Logging;
using Shared.Configuration;
using Shared.Requests.Common;
using Shared.Responses.Common;

namespace ApplicationServices.Common;

public class AddressService : IAddressService
{

    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    private readonly ICacheConfiguration<Address> _cache;
    private readonly IUnitOfWork<Guid> _unitOfWork;
    private readonly IRepositoryAsync<Address, Guid> _addressRepo;

    public AddressService(
        ICacheConfiguration<Address> cache,
        IMapper mapper,
        IUnitOfWork<Guid> unitOfWork,
        ILogger<AddressService> logger
    )
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _cache = cache;
        _addressRepo = _unitOfWork.Repository<Address>();
    }

    public async Task<ApiResponse<AddressResponse>> CreateAddress(AddressRequest request, bool isInTransaction = false)
    {
        try
        {
            request.Id = new Guid();

            // Map objects
            var addressObj = _mapper.Map<Address>(request);

            // Validate obj
            var addressValidator = await new AddressValidator().ValidateAsync(addressObj);

            // Return if invalid
            if (!addressValidator.IsValid)
            {
                return await ApiResponse<AddressResponse>.FailAsync(addressValidator.Errors, _logger);
            }

            if(!isInTransaction) _ = await _unitOfWork.StartTransaction();

            // Add record
            _ = await _addressRepo.AddAsync(addressObj);
            var response = await _unitOfWork.Save(CancellationToken.None);

            // Return if failed
            if (response <= 0)
            {
                if (!isInTransaction) await _unitOfWork.Rollback();
                return await ApiResponse<AddressResponse>.FailAsync("Failed To Save Address. Please try again later!",
                    _logger);
            }

            // Commit transaction
            if (!isInTransaction) await _unitOfWork.Commit();

            // Add the new record in cache
            _cache.SetInCacheMemoryAsync(addressObj);

            // Map post to response obj and return data
            var responseObj = _mapper.Map<AddressResponse>(addressObj);

            return await ApiResponse<AddressResponse>.SuccessAsync(responseObj);
        }
        catch (Exception e)
        {
            await _unitOfWork.Rollback();
            return await ApiResponse<AddressResponse>.FatalAsync(e, _logger);
        }
    }

}