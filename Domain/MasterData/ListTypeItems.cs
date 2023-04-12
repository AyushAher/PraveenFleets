using Domain.Common;

namespace Domain.MasterData;

public class ListTypeItem : EntityTemplate<Guid>
{
    public Guid ListTypeId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
}
