using System.Diagnostics.CodeAnalysis;
using Domain.Common;
using Enums.Documents;

namespace Domain.FileShare;

public class DocuFile_View : EntityTemplate<Guid>
{
    public string OriginalName { get; set; }

    public string Name { get; set; }

    public string? Description { get; [param: AllowNull] set; }

    public Guid OwnerId { get; set; }

    public string OwnerName { get; set; }

    public Guid ParentId { get; set; }

    public UploadType DocuType { get; set; }

    public string FileType { get; set; }

    public bool Secured { get; set; }

    public string FolderPath { get; set; }
}