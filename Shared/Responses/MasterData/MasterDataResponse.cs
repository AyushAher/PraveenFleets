namespace Shared.Responses.MasterData;

public class MasterDataResponse
{
    public Guid ListTypeId { get; set; }
    public string Code { get; set; } = string.Empty;
    public Guid Id { get; set; }
    public string ItemName { get; set; } = string.Empty;
}