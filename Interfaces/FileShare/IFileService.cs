using Enums.Documents;
using Shared.Configuration;
using Shared.Requests.Documents;
using Shared.Responses.FileShare;

namespace Interfaces.FileShare;

/// <summary>
/// Used for uploading files
/// </summary>
public interface IFileService : IService
{
    Task<ApiResponse<Guid>> UploadFileAsync(UploadRequest request);

    Task<string?> UploadFileAsync(UploadRequest request, bool handleTransaction, bool returnURL);

    Task<string?> GetFileURLAsync(Guid fileId);

    Task<string?> GetFileURLAsync(
        UploadType uploadType,
        Guid parentId,
        string fileName,
        Guid? primaryId,
        Guid? secondaryId);

    Task<FileResponse> GetFile(Guid fileId);

    Task<List<FileResponse>> GetAllOwned();

    Task<List<FileResponse>> GetAll(GetAllFilesRequest request);

    Task<bool> DeleteFileAsync(Guid fileId, bool handleTransaction);
}