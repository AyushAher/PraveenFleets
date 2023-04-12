using ApplicationServices.MappingProfile.Fileshare;
using DB.Extensions;
using Domain.Account;
using Domain.FileShare;
using Enums.Documents;
using Interfaces.Account;
using Interfaces.FileShare;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Requests.Documents;

namespace ApplicationServices.FileShare;

internal class DocumentService : IDocumentService
{
    private readonly ILogger _logger;
    private readonly IUnitOfWork<Guid> _unitOfWork;
    private readonly IRepositoryAsync<DocumentFilesUpload, Guid> _docuFilesRepo;
    private readonly ICurrentUserService _curUser;

    public DocumentService(
        ILogger<DocumentService> logger,
        IUnitOfWork<Guid> unitOfWork,
        ICurrentUserService curUserService,
        UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _docuFilesRepo = _unitOfWork.Repository<DocumentFilesUpload>();
        _curUser = curUserService;
    }

    public async Task<DocuFile_View> CreateAsync(
        Guid? docuFileId,
        string originalName,
        string fileName,
        string? description,
        string docuFileExtension,
        UploadType docuType,
        bool isSecured,
        string folderPath,
        Guid parentId,
        bool handleTransaction)
    {
        try
        {
            var docuFile1 = new DocumentFilesUpload
            {
                Id = docuFileId.HasValue ? docuFileId.Value : Guid.NewGuid(),
                OriginalName = originalName,
                Name = fileName,
                Description = description,
                OwnerId = _curUser.UserId,
                DocumentType = docuType,
                FileType = docuFileExtension,
                Secured = isSecured,
                FolderPath = folderPath,
                ParentId = parentId
            };

            var objDocument = docuFile1;
            var validationResult = new DocuFileValidator().Validate(objDocument);

            if (!validationResult.IsValid)
            {
                // ISSUE: reference to a compiler-generated method
                var list = validationResult.Errors.Select(x => x.ErrorMessage).ToList();

                foreach (var t in list) _logger.LogError(t);

                return await Task.FromException<DocuFile_View>(
                    new ApplicationException("Not a valid docufile to be created!"));

            }

            if (handleTransaction)
            {
                _ = await _unitOfWork.StartTransaction(false);
            }

            await _docuFilesRepo.AddAsync(objDocument);

            if (await _unitOfWork.Save(CancellationToken.None) <= 0)
            {
                if (handleTransaction) await _unitOfWork.Rollback();
                return await Task.FromException<DocuFile_View>(new ApplicationException("Unable to create document!"));
            }

            if (handleTransaction) await _unitOfWork.Commit();
            return handleTransaction ? await GetByIdAsync(objDocument.Id) : await GetDocument(objDocument.Id);

        }

        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Unable to create a docufile!");
            return await Task.FromException<DocuFile_View>(ex);
        }

    }

    public async Task<DocuFile_View?> GetByIdAsync(Guid docuFileId)
    {
        try
        {
            return await _unitOfWork.Repository<DocuFile_View>().Entities
                .Where(c => c.DeletedOn == new DateTime?() && c.Id == docuFileId)
                .FirstOrDefaultAsync() ?? null;
        }
        catch (Exception exception)
        {
            _logger.LogCritical(exception, "Error #12101: Unable to get a docufile!");
            return await Task.FromException<DocuFile_View>(exception);
        }
    }

    public async Task<DocuFile_View?> GetDocumentAsync(
        UploadType uploadType,
        Guid parentId,
        string fileName,
        string folderPath)
    {
        try
        {
            return await _unitOfWork.Repository<DocuFile_View>().Entities.Where(c =>
                c.DeletedOn == new DateTime?() && (int)c.DocuType == (int)uploadType && c.ParentId == parentId &&
                c.Name == fileName && c.FolderPath == folderPath).FirstOrDefaultAsync() ?? null;
        }
        catch (Exception exception)
        {
            _logger.LogCritical(exception, "Error #12102: Unable to get a docufile!");
            return await Task.FromException<DocuFile_View>(exception);
        }
    }

    public async Task<List<DocuFile_View>> GetOwnedDocs()
    {
        try
        {
            return await _unitOfWork.Repository<DocuFile_View>().Entities
                .Where(c => c.DeletedOn == new DateTime?() && c.OwnerId == _curUser.UserId)
                .ToListAsync();
        }
        catch (Exception exception)
        {
            _logger.LogCritical(exception, "Unable to get a docufile!");
            return await Task.FromException<List<DocuFile_View>>(exception);
        }
    }

    public async Task<List<DocuFile_View>> GetAllAsync(GetAllFilesRequest request)
    {
        try
        {
            List<DocuFile_View> allAsync = null;
            switch (request.OptionNo)
            {
                case 10:
                    var ownerId = request.OwnerId;
                    var userId = _curUser.UserId;
                    if ((ownerId.HasValue ? ownerId.HasValue ? ownerId.GetValueOrDefault() == userId ? 1 : 0 : 1 : 0) !=
                        0)
                        return await GetOwnedDocs();
                    break;
                case 15:
                    if (request.OwnerId.HasValue && request.OwnerId.Value != Guid.Empty)
                    {
                        allAsync = await _unitOfWork.Repository<DocuFile_View>().Entities
                            .Where(c => c.DeletedOn == new DateTime?() && c.OwnerId == request.OwnerId)
                            .ToListAsync();
                        break;
                    }

                    break;
                case 100:
                    if (request.ParentId.HasValue && request.ParentId.Value != Guid.Empty)
                    {
                        allAsync = await _unitOfWork.Repository<DocuFile_View>().Entities
                            .Where(c => c.DeletedOn == new DateTime?() && c.ParentId == request.ParentId.Value)
                            .ToListAsync();
                        break;
                    }

                    break;
                case 110:
                    if (request.ParentId.HasValue && request.ParentId.Value != Guid.Empty && request.DocuType.HasValue)
                    {
                        allAsync = await _unitOfWork.Repository<DocuFile_View>().Entities
                            .Where(c =>
                                c.DeletedOn == new DateTime?() && c.ParentId == request.ParentId.Value &&
                                (int?)c.DocuType == (int?)request.DocuType).ToListAsync();
                        break;
                    }

                    break;
                case 200:
                    if (request.CompanyId.HasValue && request.CompanyId.Value != Guid.Empty)
                    {
                        allAsync = await _unitOfWork.Repository<DocuFile_View>().Entities
                            .Where(c => c.DeletedOn == new DateTime?())
                            .ToListAsync();
                        break;
                    }

                    break;
                case 1000:
                    if (request.DocuType.HasValue && request.DocuType.Value > 0)
                    {
                        allAsync = await _unitOfWork.Repository<DocuFile_View>().Entities
                            .Where(c =>
                                c.DeletedOn == new DateTime?() && (int)c.DocuType == (int)request.DocuType.Value)
                            .ToListAsync();
                        break;
                    }

                    break;
            }

            return allAsync;
        }
        catch (Exception exception)
        {
            _logger.LogCritical(exception, "Error #12103: Unable to get a docufile!");
            return await Task.FromException<List<DocuFile_View>>(exception);
        }
    }

    public async Task<bool> DeleteAsync(Guid docuFileId, bool handleTransaction)
    {
        try
        {
            if (handleTransaction)
            {
                _ = await _unitOfWork.StartTransaction(false);
            }

            var blnStatus = await _docuFilesRepo.DeleteAsync(await _docuFilesRepo.GetByIdAsync(docuFileId));
            blnStatus = await _unitOfWork.Save(CancellationToken.None) > 0 & blnStatus;
            if (handleTransaction)
            {
                if (blnStatus)
                    await _unitOfWork.Commit();
                else
                    await _unitOfWork.Rollback();
            }

            if (!blnStatus)
                _logger.LogError("Unable to delete the document!");
            return blnStatus;
        }
        catch (Exception exception)
        {
            _logger.LogCritical(exception, "Unable to delete a docufile!");
            return await Task.FromException<bool>(exception);
        }
    }

    private async Task<DocuFile_View?> GetDocument(Guid docuFileId)
    {
        try
        {
            var byIdAsync = await _docuFilesRepo.GetByIdAsync(docuFileId);

            if (byIdAsync == null) return null;

            var docuFileView = new DocuFile_View
            {
                Id = byIdAsync.Id,
                CreatedBy = byIdAsync.CreatedBy,
                CreatedOn = byIdAsync.CreatedOn,
                LastModifiedBy = byIdAsync.LastModifiedBy,
                LastModifiedOn = byIdAsync.LastModifiedOn,
                DeletedBy = byIdAsync.DeletedBy,
                DeletedOn = byIdAsync.DeletedOn,
                OriginalName = byIdAsync.OriginalName,
                Name = byIdAsync.Name,
                Description = byIdAsync.Description,
                OwnerId = byIdAsync.OwnerId,
                DocuType = byIdAsync.DocumentType,
                FileType = byIdAsync.FileType,
                Secured = byIdAsync.Secured,
                FolderPath = byIdAsync.FolderPath,
                ParentId = byIdAsync.ParentId,
                OwnerName = _curUser.FullName
            };

            return docuFileView;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error #12103: Unable to get a docufile!");
            return null;
        }
    }
}
