namespace Shared.Responses.MasterData;

public class ListTypeItemResponse
{
    public Guid ListTypeId { get; set; }
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
}
