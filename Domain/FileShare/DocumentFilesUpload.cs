using System.ComponentModel.DataAnnotations;
using Domain.Common;
using Enums.Documents;

namespace Domain.FileShare;

public class DocumentFilesUpload : EntityTemplate<Guid>
{
    [Required]
    [MaxLength(100)]
    public string OriginalName { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public Guid OwnerId { get; set; }

    [Required]
    public Guid ParentId { get; set; }

    [Required]
    public UploadType DocumentType { get; set; }

    [Required]
    [MaxLength(10)]
    public string FileType { get; set; }

    [Required]
    public bool Secured { get; set; }

    [Required]
    [MaxLength(500)]
    public string FolderPath { get; set; }
}