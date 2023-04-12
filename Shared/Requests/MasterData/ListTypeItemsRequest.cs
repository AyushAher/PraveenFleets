﻿namespace Shared.Requests.MasterData;

public class ListTypeItemRequest
{
    public Guid Id { get; set; }
    public Guid ListTypeId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
}
