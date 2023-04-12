namespace Shared.Requests.Common;

public class AddressRequest
{
    public Guid ParentId { get; set; }
    public Guid Id { get; set; }
    public string Attention { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string AddressLine2 { get; set; } = string.Empty;
    public string Tal { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Pincode { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public bool IsShippingAddress { get; set; }
    public bool IsBillingAddress { get; set; }
}
