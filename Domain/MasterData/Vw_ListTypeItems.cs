using Domain.Common;

namespace Domain.MasterData;

public class Vw_ListTypeItems : EntityTemplate<Guid>
{
    public string ListCode { get; set; } = string.Empty;
    public Guid ListTypeId { get; set; }
    public string ListName { get; set; } = string.Empty;
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public bool IsMaster { get; set; }
}