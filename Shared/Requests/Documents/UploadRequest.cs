using System.ComponentModel.DataAnnotations;
using Enums.Documents;

namespace Shared.Requests.Documents;

public class UploadRequest
{
    [Required] public string FileName { get; set; } = string.Empty;

    [Required]
    public string Extension { get; set; } = string.Empty;

    [Required]
    public UploadType UploadType { get; set; }

    [Required]
    public string Data { get; set; } = string.Empty;

    [Required]
    public Guid ParentId { get; set; }

    public Guid? PrimaryId { get; set; }

    public Guid? SecondaryId { get; set; }
}