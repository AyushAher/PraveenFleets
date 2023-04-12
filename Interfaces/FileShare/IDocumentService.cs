using Domain.FileShare;
using Enums.Documents;
using Shared.Requests.Documents;

namespace Interfaces.FileShare;

/// <summary>
/// Used for DB management
/// </summary>
public interface IDocumentService : IService
{
    public Task<DocuFile_View?> GetByIdAsync(Guid docuFileId);


    public Task<DocuFile_View> CreateAsync(
        Guid? docuFileId,
        string originalName,
        string fileName,
        string? description,
        string docuFileExtension,
        UploadType docuType,
        bool isSecured,
        string folderPath,
        Guid parentId,
        bool handleTransaction);

    public Task<DocuFile_View?> GetDocumentAsync(
        UploadType uploadType,
        Guid parentId,
        string fileName,
        string folderPath);

    public Task<List<DocuFile_View>> GetOwnedDocs();
    public Task<bool> DeleteAsync(Guid docuFileId, bool handleTransaction);
    public Task<List<DocuFile_View>> GetAllAsync(GetAllFilesRequest request);
}