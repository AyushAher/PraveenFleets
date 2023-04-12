using Domain.FileShare;
using FluentValidation;

namespace ApplicationServices.MappingProfile.Fileshare;


public class DocuFileValidator : AbstractValidator<DocumentFilesUpload>
{
    public DocuFileValidator()
    {
        RuleFor(p => p.OriginalName)
            .NotEmpty()
            .WithMessage("The document/file name is required.")
            .MaximumLength(100)
            .WithMessage("Document/File Name must be at max 100 characters long.");


        RuleFor(p => p.Description)
            .MaximumLength(500)
            .WithMessage("Document/File Description must be at max 500 characters long.");

        RuleFor(p => p.FileType)
            .NotEmpty()
            .WithMessage("The file type is required.")
            .MaximumLength(10)
            .WithMessage("File type must be at max 10 characters long.");

        RuleFor(p => p.DocumentType)
            .NotNull()
            .WithMessage("The document type is required!");

        RuleFor(p => p.OwnerId)
            .NotNull()
            .WithMessage("The Owner Information is required!");

        RuleFor(p => p.Secured)
            .NotNull()
            .WithMessage("Secured or not has to be indicated!");

        RuleFor(p => p.ParentId)
            .NotNull()
            .WithMessage("The Parent Information is required!");

        RuleFor(p => p.FolderPath)
            .NotEmpty()
            .WithMessage("The document/file path is required.")
            .MaximumLength(500)
            .WithMessage("Document/File path must be at max 500 characters long.");
    }
}