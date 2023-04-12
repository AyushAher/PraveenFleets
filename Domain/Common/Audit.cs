namespace Domain.Common;

public class Audit : IEntity<long>
{
    public long Id { get; set; }

    public Guid UserId { get; set; } = Guid.Empty;

    public string Type { get; set; } = string.Empty;

    public string TableName { get; set; } = string.Empty;

    public DateTime DateTime { get; set; } = DateTime.MinValue;

    public string OldValues { get; set; } = string.Empty;

    public string NewValues { get; set; } = string.Empty;

    public string AffectedColumns { get; set; } = string.Empty;

    public string PrimaryKey { get; set; } = string.Empty;
}
