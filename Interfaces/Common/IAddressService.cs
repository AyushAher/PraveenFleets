using Shared.Configuration;
using Shared.Requests.Common;
using Shared.Responses.Common;

namespace Interfaces.Common;

public interface IAddressService
{
    Task<ApiResponse<AddressResponse>> CreateAddress(AddressRequest request,bool isInTransaction = false);
    
    Task<ApiResponse<AddressResponse>> GetAddressByParentId(Guid parentId);
}