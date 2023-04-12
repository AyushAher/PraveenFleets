using AutoMapper;
using Domain.FileShare;
using Shared.Responses.FileShare;

namespace ApplicationServices.MappingProfile.Fileshare;

public class FileShareMapper : Profile
{
    public FileShareMapper()
    {
        CreateMap<DocuFile_View, FileResponse>()
            .ReverseMap();
    }
}