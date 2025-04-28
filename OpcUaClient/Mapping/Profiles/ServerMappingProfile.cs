using AutoMapper;
using OpcUaClient.Domain.Models;
using OpcUaClient.Models.ServerModels;
using OpcUaClient.Models.TagModels;

namespace OpcUaClient.Mapping.Profiles;

public class ServerMappingProfile : Profile
{
    public ServerMappingProfile()
    {
        CreateMap<Server, ServerViewModel>();
        CreateMap<ServerCreateModel, Server>();
        CreateMap<ServerUpdateModel, Server>();
    }
}