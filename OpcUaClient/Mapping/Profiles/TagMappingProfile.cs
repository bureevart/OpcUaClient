using AutoMapper;
using OpcUaClient.Domain.Models;
using OpcUaClient.Models.TagModels;

namespace OpcUaClient.Mapping.Profiles;

public class TagMappingProfile : Profile
{
    public TagMappingProfile()
    {
        CreateMap<Tag, TagViewModel>();
        CreateMap<Tag, TagShortViewModel>();
        CreateMap<TagCreateModel, Tag>();
        CreateMap<TagUpdateModel, Tag>();
    }
}