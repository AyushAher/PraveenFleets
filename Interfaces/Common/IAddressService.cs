using Shared.Configuration;
using Shared.Requests.Common;
using Shared.Responses.Common;

namespace Interfaces.Common;

public interface IAddressService
{
    public Task<ApiResponse<AddressResponse>> CreateAddress(AddressRequest request);
}