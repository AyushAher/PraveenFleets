namespace Shared.Responses.MasterData;

public class Vw_ListTypeItemsResponse
{

    public Guid Id { get; set; }
    public string ListCode { get; set; } = string.Empty;
    public Guid ListTypeId { get; set; }
    public string ListName { get; set; } = string.Empty;
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public bool IsMaster { get; set; }
}