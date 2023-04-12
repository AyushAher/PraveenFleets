namespace Shared.Requests.MasterData;

public class ListTypeRequest
{
    public string Code { get; set; } = string.Empty;
    public Guid Id { get; set; }
    public string ListName { get; set; } = string.Empty;
}
