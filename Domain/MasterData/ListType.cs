using Domain.Common;

namespace Domain.MasterData;

public class ListType : EntityTemplate<Guid>
{
    public string Code { get; set; } = string.Empty;
    public string ListName { get; set; } = string.Empty;
}
