using Shared.Configuration;
using Shared.Requests.MasterData;
using Shared.Responses.MasterData;

namespace Interfaces.MasterData;

public interface IMasterDataService : IService
{
    public Task<ApiResponse<ListTypeResponse>> CreateNewList(ListTypeRequest request);

    public Task<ApiResponse<ListTypeItemResponse>> CreateNewListItem(ListTypeItemRequest request);

    public Task<ApiResponse<Vw_ListTypeItemsResponse>> GetMasterDataByItemId(Guid itemId);

    public Task<ApiResponse<List<Vw_ListTypeItemsResponse>>> GetMasterDataByListCode(string listCode);
}