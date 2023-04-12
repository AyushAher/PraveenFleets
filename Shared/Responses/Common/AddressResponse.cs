namespace Shared.Responses.Common;

public class AddressResponse
{
    public Guid ParentId { get; set; }
    public string Attention { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public Guid Id { get; set; }
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
