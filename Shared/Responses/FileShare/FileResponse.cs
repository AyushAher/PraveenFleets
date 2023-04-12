using System.Diagnostics.CodeAnalysis;
using Enums.Documents;

namespace Shared.Responses.FileShare;

public class FileResponse
{

    public Guid Id { get; set; }

    public DateTime CreatedOn { get; set; }

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