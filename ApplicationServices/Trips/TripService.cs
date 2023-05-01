using ApplicationServices.Organizations;
using AutoMapper;
using DB.Extensions;
using Domain.Organization;
using Domain.Trips;
using Enums.Trips;
using Interfaces.Account;
using Interfaces.Common;
using Interfaces.Organizations;
using Interfaces.Trips;
using Microsoft.Extensions.Logging;
using Shared.Configuration;
using Shared.Requests.Trips;
using Shared.Responses.Organization;
using Shared.Responses.Trips;
using Utility.Email;

namespace ApplicationServices.Trips;

public class TripService : ITripService
{

    private readonly ILogger<TripService> _logger;
    private readonly IUnitOfWork<Guid> _unitOfWork;
    private readonly IRepositoryAsync<Trip, Guid> _tripRepo;
    private readonly ICacheConfiguration<Trip> _cache;
    private readonly IAddressService _addressService;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    
    private readonly IEMailService _eMailService;
    private readonly IMailGenerator _mailGenerator;

    public TripService(
        IMapper mapper,
        ILogger<TripService> logger,
        IUnitOfWork<Guid> unitOfWork,
        ICacheConfiguration<Trip> cache,
        IAddressService addressService,
        IUserService userService,
        ICurrentUserService currentUserService,
        IEMailService eMailService,
        IMailGenerator mailGenerator
    )
    {
        _mapper = mapper;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _tripRepo = unitOfWork.Repository<Trip>();
        _cache = cache;
        _addressService = addressService;
        _userService = userService;
        _currentUserService = currentUserService;
        _mailGenerator = mailGenerator;
        _eMailService = eMailService;
    }

    public async Task<ApiResponse<ScheduleTripResponse>> SaveTripDraft(ScheduleTripRequest request)
    {
        try
        {
            var tripId = Guid.NewGuid();
            // Check if user exists in or. with that email
            var passengerOrgEmployeeRecord = _unitOfWork.Repository<OrganizationEmployee>().Entities
                .FirstOrDefault(x => x.Email == request.PassengerEmailId);

            if (passengerOrgEmployeeRecord == null)
            {
                _logger.LogError("No employee found with the email provided");
                return await ApiResponse<ScheduleTripResponse>.FailAsync("No employee found with the email provided",
                    _logger);
            }

            var transaction = await _unitOfWork.StartTransaction();

            // Map trip id
            request.DropAddress.ParentId = tripId;
            request.PickUpAddress.ParentId = tripId;

            // save pickup address
            var pickupAddressRequest = await _addressService.CreateAddress(request.PickUpAddress, true);
            if (pickupAddressRequest.Failed)
            {
                await transaction.RollbackAsync();
                _logger.LogError("Error saving Pickup Address");
                return await ApiResponse<ScheduleTripResponse>.FailAsync(pickupAddressRequest.Messages,
                    _logger);
            }

            // save drop address
            var dropAddressRequest = await _addressService.CreateAddress(request.DropAddress, true);
            if (dropAddressRequest.Failed)
            {
                await transaction.RollbackAsync();
                _logger.LogError("Error saving Pickup Address");
                return await ApiResponse<ScheduleTripResponse>.FailAsync(dropAddressRequest.Messages,
                    _logger);
            }
            

            var mappedObj = _mapper.Map<Trip>(request);
            mappedObj.Id = tripId;
            mappedObj.DropAddressId = dropAddressRequest.Data.Id;
            mappedObj.PickUpAddressId = pickupAddressRequest.Data.Id;
            mappedObj.PassengerUserId = passengerOrgEmployeeRecord.UserId;

            // set status to draft
            mappedObj.Status = TripStatus.Draft;

            // Add record
            _ = await _tripRepo.AddAsync(mappedObj);
            var response = await _unitOfWork.Save(CancellationToken.None);

            // Return if failed
            if (response <= 0)
            {
                await _unitOfWork.Rollback();
                return await ApiResponse<ScheduleTripResponse>.FailAsync(
                    "Failed To Save Organization. Please try again later!",
                    _logger);
            }

            // Commit transaction
            await transaction.CommitAsync();

            // Set record in cache
            _cache.SetInCacheMemoryAsync(mappedObj);

            var data = _mapper.Map<ScheduleTripResponse>(mappedObj);
            return await ApiResponse<ScheduleTripResponse>.SuccessAsync(data);
        }
        catch (Exception e)
        {
            return await ApiResponse<ScheduleTripResponse>.FatalAsync(e, _logger);
        }
    }
}