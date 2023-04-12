using System.Net;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using AutoMapper;
using DB.Extensions;
using Domain.FileShare;
using Enums.Documents;
using Interfaces.Account;
using Interfaces.FileShare;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Configuration;
using Shared.Requests.Documents;
using Shared.Responses.FileShare;
using Utility.Extensions;

namespace Utility.Storage;

public sealed class FileService : IFileService
{

    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly AWSS3Config _awsConfig;
    private readonly RegionEndpoint _regionName;
    private readonly IAmazonS3 _awsS3Client;
    private readonly IDocumentService _docuFileService;
    private readonly ICurrentUserService _userService;
    private readonly IUnitOfWork<Guid> _unitOfWork;


    public FileService(
        ILogger<FileService> logger,
        IOptions<AWSS3Config> awsConfig,
        ICurrentUserService curUserService,
        IUnitOfWork<Guid> unitOfWork,
        IDocumentService docuFileService,
        IMapper mapper)
    {
        _logger = logger;
        _awsConfig = awsConfig.Value;
        _userService = curUserService;
        _mapper = mapper;
        _regionName = RegionEndpoint.GetBySystemName(_awsConfig.Region);
        _unitOfWork = unitOfWork;
        _docuFileService = docuFileService;
        _awsS3Client = new AmazonS3Client(_awsConfig.AWSAccessKey, _awsConfig.AWSSecretAccessKey, _regionName);
    }

    public async Task<ApiResponse<Guid>> UploadFileAsync(UploadRequest request)
    {

        try
        {
            if (!UploadFileAsync(request, false, out var documentInfo, out var errorInfo))
                return await ApiResponse<Guid>.FailAsync(errorInfo, _logger);
            return documentInfo == null
                ? await ApiResponse<Guid>.FailAsync("Unknown Error, Please contact Support!", _logger)
                : await ApiResponse<Guid>.SuccessAsync(documentInfo.Id);
        }
        catch (Exception ex)
        {
            return await ApiResponse<Guid>.FatalAsync(ex, _logger);
        }
    }

    public async Task<string?> UploadFileAsync(
        UploadRequest request,
        bool handleTransaction,
        bool returnURL)
    {
        try
        {
            DocuFile_View documentInfo;
            string errorInfo;
            return UploadFileAsync(request, handleTransaction, out documentInfo, out errorInfo)
                ? documentInfo != null
                    ? !returnURL
                        ? documentInfo.Id.ToString()
                        : !documentInfo.Secured
                            ? GetPublicFileURL(documentInfo.FolderPath, documentInfo.Name)
                            : GetPrivateFileURL(documentInfo.FolderPath, documentInfo.Name)
                    : "$$E:Unknow Error (Code 12002). Please contact Support!"
                : "$$E:" + errorInfo;
        }
        catch (Exception ex)
        {
            return "$$E:" + ex.Message;
        }
    }

    public async Task<FileResponse> GetFile(Guid fileId)
    {
        try
        {
            return _mapper.Map<FileResponse>(await _docuFileService.GetByIdAsync(fileId));
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error: Unable to get a record!");
            return await Task.FromException<FileResponse>(ex);
        }
    }

    public async Task<string?> GetFileURLAsync(Guid fileId)
    {
        try
        {
            DocuFile_View byIdAsync = await _docuFileService.GetByIdAsync(fileId);
            return byIdAsync != null
                ? GetPrivateFileURL(byIdAsync.FolderPath, byIdAsync.Name)
                : null;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error #12200.2: Unable to get a docufile!");
            return await Task.FromException<string>(ex);
        }
    }

    public async Task<string?> GetFileURLAsync(
        UploadType uploadType,
        Guid parentId,
        string fileName,
        Guid? primaryId,
        Guid? secondaryId)
    {
        try
        {
            string folderPath;
            GetDocFileDetails(uploadType, primaryId, secondaryId, out var _, out var _, out folderPath);

            DocuFile_View documentAsync =
                await _docuFileService.GetDocumentAsync(uploadType, parentId, fileName, folderPath);

            return documentAsync != null
                ? !documentAsync.Secured
                    ? GetPublicFileURL(documentAsync.FolderPath, documentAsync.Name)
                    : GetPrivateFileURL(documentAsync.FolderPath, documentAsync.Name)
                : null;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error #12200.3: Unable to get a docufile!");
            return await Task.FromException<string>(ex);
        }
    }

    public async Task<List<FileResponse>> GetAllOwned()
    {
        try
        {
            return _mapper.Map<List<FileResponse>>(await _docuFileService.GetOwnedDocs());
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error #12200.4: Unable to delete a docufile!");
            return null;
        }
    }

    public async Task<List<FileResponse>> GetAll(GetAllFilesRequest request)
    {
        try
        {
            return _mapper.Map<List<FileResponse>>(await _docuFileService.GetAllAsync(request));
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error #12200.5: Unable to delete a docufile!");
            return null;
        }
    }

    public async Task<bool> DeleteFileAsync(Guid fileId, bool handleTransaction)
    {
        try
        {
            DocuFile_View byIdAsync = await _docuFileService.GetByIdAsync(fileId);
            DeleteObjectRequest request = new()
            {
                BucketName = byIdAsync.Secured ? _awsConfig.PrivateBucket : _awsConfig.PrivateBucket,
                Key = byIdAsync.FolderPath + "/" + byIdAsync.Name
            };
            _logger.LogTrace("Delete document / object from AWS S3");
            await _awsS3Client.DeleteObjectAsync(request);
            return await _docuFileService.DeleteAsync(fileId, handleTransaction);
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogCritical(ex, "Error #12200.6: Unable to delete a docufile!");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error #12200.7: Unable to delete a docufile!");
            return false;
        }
    }

    private bool UploadFileAsync(
        UploadRequest request,
        bool handleTransaction,
        out DocuFile_View? documentInfo,
        out string errorInfo)
    {
        var guid = Guid.NewGuid();
        errorInfo = string.Empty;
        try
        {
            if (string.IsNullOrEmpty(request.Data))
            {
                errorInfo = "No data present for creating document!";
                documentInfo = null;
                _logger.LogError(errorInfo);
                return false;
            }

            var byteData = Convert.FromBase64String(request.Data);

            MemoryStream memoryStream = new(byteData);
            if (memoryStream.Length > 0L)
            {
                if (request.Extension.Substring(0, 1) == ".")
                    request.Extension = request.Extension.Substring(1);

                request.FileName = WebUtility.HtmlEncode(request.FileName);
                request.Extension = WebUtility.HtmlEncode(request.Extension);

                var fileName = request.FileName?.Trim('"') + "_" + guid + "." + request.Extension.Trim();

                GetDocFileDetails(request.UploadType, request.PrimaryId, request.SecondaryId, out var bucketName,
                    out var isPublic, out var folderPath);

                TransferUtilityUploadRequest request1 = new()
                {
                    InputStream = memoryStream,
                    Key = folderPath + "/" + fileName,
                    BucketName = bucketName
                };

                if (isPublic) request1.CannedACL = S3CannedACL.PublicRead;

                new TransferUtility(_awsS3Client).Upload(request1);

                documentInfo = _docuFileService.CreateAsync(guid, request.FileName, fileName,
                    null, request.Extension.Trim(), request.UploadType, !isPublic, folderPath, request.ParentId,
                    handleTransaction).Result;

            }
            else
            {
                errorInfo = "No data present for creating document!";
                documentInfo = null;
                _logger.LogError(errorInfo);
                return false;
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            errorInfo = ex.Message;
            documentInfo = null;
            _logger.LogError(errorInfo);
            return false;
        }

        return true;
    }

    private void GetDocFileDetails(
        UploadType uploadType,
        Guid? primaryId,
        Guid? secondaryId,
        out string bucketName,
        out bool isPublic,
        out string folderPath)
    {
        bucketName = _awsConfig.PublicBucket;
        isPublic = false;
        folderPath = uploadType.ToDescriptionString().Replace("\\", "/");

    }

    private string GetPublicFileURL(string folderPath, string fileName) =>
        $"https://{_awsConfig.PublicBucket}.s3.{_regionName.SystemName}.amazonaws.com/{folderPath}/{fileName}";

    private string GetPrivateFileURL(string folderPath, string fileName)
    {
        try
        {
            return _awsS3Client.GetPreSignedURL(new()
            {
                BucketName = _awsConfig.PrivateBucket,
                Key = folderPath + "/" + fileName,
                Expires = DateTime.UtcNow.AddHours(_awsConfig.AWSSessionTimeOut)
            });
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogCritical(ex, "Error #12200.8: Unable to get a docufile Url!");
            return string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error #12200.9: Unable to get a docufile Url!");
            return string.Empty;
        }
    }
}