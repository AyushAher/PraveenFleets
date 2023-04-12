using Enums.Documents;

namespace Shared.Requests.Documents;

public class GetAllFilesRequest
{

    public int OptionNo { get; set; }

    public Guid? OwnerId { get; set; }

    public Guid? ParentId { get; set; }

    public Guid? CompanyId { get; set; }

    public UploadType? DocuType { get; set; }

}