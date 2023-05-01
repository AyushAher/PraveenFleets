using Interfaces.MasterData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Configuration;
using Shared.Requests.MasterData;
using Shared.Responses.MasterData;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MasterDataController : ControllerBase
{
    private readonly IMasterDataService _masterDataService;

    public MasterDataController(IMasterDataService masterDataService)
        => _masterDataService = masterDataService;

    [HttpGet("GetByListCode/{listCode}")]
    public async Task<ApiResponse<List<Vw_ListTypeItemsResponse>>> GetMasterDataByListCode(string listCode)
        => await _masterDataService.GetMasterDataByListCode(listCode);

    [HttpGet("GetByItemId/{itemId}")]
    public async Task<ApiResponse<Vw_ListTypeItemsResponse>> GetMasterDataByItemId(string itemId)
        => await _masterDataService.GetMasterDataByItemId(Guid.Parse(itemId));

    [HttpPost("CreateNewList")]
    public async Task<ApiResponse<ListTypeResponse>> CreateNewList(ListTypeRequest request)
        => await _masterDataService.CreateNewList(request);

    [HttpPost("CreateNewListItem")]
    public async Task<ApiResponse<ListTypeItemResponse>> CreateNewListItem(ListTypeItemRequest request)
        => await _masterDataService.CreateNewListItem(request);
}