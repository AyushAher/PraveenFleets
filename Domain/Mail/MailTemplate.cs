using System.ComponentModel.DataAnnotations;
using Domain.Common;

namespace Domain.Mail;

public class MailTemplate : EntityTemplate<Guid>
{
    [Required][MaxLength(50)] public string Code { get; set; } = string.Empty;

    [Required] public int Version { get; set; } = 1;

    [Required][MaxLength(500)] public string Subject { get; set; } = string.Empty;

    [Required] public string Body { get; set; } = string.Empty;
}

