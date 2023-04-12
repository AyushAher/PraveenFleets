using System.Text.Json;
using Enums.Common;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Domain.Common;

public class AuditEntry
{

    public EntityEntry Entry { get; }

    public Guid UserId { get; set; }

    public string TableName { get; set; }

    public Dictionary<string, object> KeyValues { get; } = new();

    public Dictionary<string, object> OldValues { get; } = new();

    public Dictionary<string, object> NewValues { get; } = new();

    public List<PropertyEntry> TemporaryProperties { get; } = new();

    public AuditType AuditType { get; set; }

    public List<string> ChangedColumns { get; } = new();

    public bool HasTemporaryProperties => TemporaryProperties.Any();

    public AuditEntry(EntityEntry entry) => Entry = entry;

    public Audit ToAudit() => new()
    {
        UserId = UserId,
        Type = AuditType.ToString(),
        TableName = TableName,
        DateTime = DateTime.UtcNow,
        PrimaryKey = JsonSerializer.Serialize(KeyValues),

        OldValues = OldValues.Count == 0
            ? null
            : JsonSerializer.Serialize(OldValues),

        NewValues = NewValues.Count == 0
            ? null
            : JsonSerializer.Serialize(NewValues),

        AffectedColumns = ChangedColumns.Count == 0
            ? null
            : JsonSerializer.Serialize(ChangedColumns)
    };

}