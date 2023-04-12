using System.Text;
using ApplicationServices.MappingProfile.MasterData;
using AutoMapper;
using DB.Extensions;
using Domain.MasterData;
using Interfaces.MasterData;
using Microsoft.Extensions.Logging;
using Shared.Configuration;
using Shared.Requests.MasterData;
using Shared.Responses.MasterData;

namespace ApplicationServices.MasterData;


public class MasterDataService : IMasterDataService
{
    private readonly IMapper _mapper; // Dependency for object mapping
    private readonly ILogger _logger; // Dependency for logging
    private readonly IUnitOfWork<Guid> _unitOfWork; // Dependency for Unit of Work
    private readonly IRepositoryAsync<ListType, Guid> _listTypeRepo; // Dependency for List type repository
    private readonly IRepositoryAsync<ListTypeItem, Guid> _listTypeItemRepo; // Dependency for List Type Items repository
    private readonly ICacheConfiguration<ListTypeItem> _lstTypeItemCache;
    private readonly ICacheConfiguration<ListType> _lstTypeCache;

    public MasterDataService(
        ICacheConfiguration<ListTypeItem> lstTypeItemCache,
        ICacheConfiguration<ListType> lstTypeCache,
        IMapper mapper,
        IUnitOfWork<Guid> unitOfWork,
        ILogger<MasterDataService> logger
    )
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;

        _lstTypeItemCache = lstTypeItemCache;
        _lstTypeCache = lstTypeCache;
        _listTypeItemRepo = _unitOfWork.Repository<ListTypeItem>(); // Initializes List type Item repository
        _listTypeRepo = _unitOfWork.Repository<ListType>(); // Initializes List type repository
    }

    public async Task<ApiResponse<ListTypeItemResponse>> CreateNewListItem(ListTypeItemRequest request)
    {
        try
        {
            var mappedRequestObj = _mapper.Map<ListTypeItem>(request);

            var listTypeItemValidators = new ListTypeItemValidators();
            var validationResultObj = await listTypeItemValidators.ValidateAsync(mappedRequestObj);

            if (!validationResultObj.IsValid)
            {
                return await ApiResponse<ListTypeItemResponse>.FailAsync(validationResultObj.Errors, _logger);
            }

            mappedRequestObj.Id = Guid.NewGuid();

            await _unitOfWork.StartTransaction();

            // Add record
            _ = await _listTypeItemRepo.AddAsync(mappedRequestObj);
            var response = await _unitOfWork.Save(CancellationToken.None);

            // Return if failed
            if (response <= 0)
            {
                await _unitOfWork.Rollback(); // Rolls back the transaction
                return await ApiResponse<ListTypeItemResponse>.FailAsync(
                    "Failed to save List Type. Please try again later!", _logger);
            }

            // Set in cache
            _lstTypeItemCache.SetInCacheMemoryAsync(mappedRequestObj);

            // Commit transaction
            await _unitOfWork.Commit(); // Commits the transaction

            // Map post to response obj and return data
            var responseObj = _mapper.Map<ListTypeItemResponse>(mappedRequestObj);

            return await ApiResponse<ListTypeItemResponse>.SuccessAsync(responseObj);
        }
        catch (Exception e)
        {
            return await ApiResponse<ListTypeItemResponse>.FatalAsync(e, _logger);
        }
    }

    public async Task<ApiResponse<ListTypeResponse>> CreateNewList(ListTypeRequest request)
    {
        try
        {
            var mappedRequestObj = _mapper.Map<ListType>(request);

            var listTypeValidatorObj = new ListTypeValidators();
            var validationResultObj = await listTypeValidatorObj.ValidateAsync(mappedRequestObj);

            if (!validationResultObj.IsValid)
            {
                return await ApiResponse<ListTypeResponse>.FailAsync(validationResultObj.Errors, _logger);
            }

            mappedRequestObj.Id = Guid.NewGuid();

            await _unitOfWork.StartTransaction();

            // Add record
            _ = await _listTypeRepo.AddAsync(mappedRequestObj);
            var response = await _unitOfWork.Save(CancellationToken.None);

            // Return if failed
            if (response <= 0)
            {
                await _unitOfWork.Rollback(); // Rolls back the transaction
                return await ApiResponse<ListTypeResponse>.FailAsync(
                    "Failed to save List Type. Please try again later!", _logger);
            }

            // Set in cache
            _lstTypeCache.SetInCacheMemoryAsync(mappedRequestObj);

            // Commit transaction
            await _unitOfWork.Commit(); // Commits the transaction

            // Map post to response obj and return data
            var responseObj = _mapper.Map<ListTypeResponse>(mappedRequestObj);

            return await ApiResponse<ListTypeResponse>.SuccessAsync(responseObj);
        }
        catch (Exception e)
        {
            return await ApiResponse<ListTypeResponse>.FatalAsync(e, _logger);
        }
    }

    public async Task<ApiResponse<Vw_ListTypeItemsResponse>> GetMasterDataByItemId(Guid itemId)
    {
        try
        {
            var vwListTypeItemObj = await _unitOfWork.Repository<Vw_ListTypeItems>()
                                        .GetByIdAsync(itemId);

            var responseObj = _mapper.Map<Vw_ListTypeItemsResponse>(vwListTypeItemObj);

            return await ApiResponse<Vw_ListTypeItemsResponse>.SuccessAsync(responseObj);
        }
        catch (Exception e)
        {
            return await ApiResponse<Vw_ListTypeItemsResponse>.FatalAsync(e, _logger);
        }
    }

    public async Task<ApiResponse<List<Vw_ListTypeItemsResponse>>> GetMasterDataByListCode(string listCode)
    {
        try
        {
            var masterDataRequest = new GetViewQueryRequest
            {
                CheckListCode = true,
                ParamStr = listCode
            };

            var vwListTypeItemObj = await _unitOfWork.Repository<Vw_ListTypeItems>()
                .GetAllAsync(GetViewQuery(masterDataRequest));

            var responseObj = _mapper.Map<List<Vw_ListTypeItemsResponse>>(vwListTypeItemObj);

            return await ApiResponse<List<Vw_ListTypeItemsResponse>>.SuccessAsync(responseObj);
        }
        catch (Exception e)
        {
            return await ApiResponse<List<Vw_ListTypeItemsResponse>>.FatalAsync(e, _logger);
        }
    }

    private static string GetViewQuery(GetViewQueryRequest request)
    {
        var queryStr = new StringBuilder();
        queryStr.Append($"(select * from `vw_listitems` where (`vw_listitems`.`DeletedBy` is null)");

        if (request.CheckCode)
            queryStr.Append($"and (`vw_listitems`.`ItemCode`=\"{request.ParamStr}\" )");

        if (request.CheckListId)
            queryStr.Append($"and (`vw_listitems`.`ListTypeId`=\"{request.ParamStr}\" )");


        if (request.CheckListCode)
            queryStr.Append($"and (`vw_listitems`.`ListCode`=\"{request.ParamStr}\" )");

        if (request.CheckItemId)
            queryStr.Append($"and (`vw_listitems`.`Id`=\"{request.ParamStr}\" )");

        queryStr.Append(");");
        return queryStr.ToString();
    }

    internal class GetViewQueryRequest
    {
        public Guid ItemId { get; set; }
        public Guid ListId { get; set; }
        public string ParamStr { get; set; }
        public bool CheckCode { get; set; }
        public bool CheckListCode { get; set; }
        public bool CheckListId { get; set; }
        public bool CheckItemId { get; set; }

    }
}