using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OpcUaClient.Domain.Interfaces.Providers;
using OpcUaClient.Domain.Models;
using OpcUaClient.Models.ServerModels;

namespace OpcUaClient.Controllers;

public class ServerController(
    IServerCrudProvider crudProvider,
    IMapper mapper,
    IValidator<Server> validator
) : BaseEntityController<Server>
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ServerViewModel>>> GetAll()
    {
        var result = await crudProvider.GetAll();
        return mapper.Map<List<ServerViewModel>>(result);
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ServerViewModel>> Get(Guid id)
    {
        var result = await crudProvider.Get(id);
        return mapper.Map<ServerViewModel>(result);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Guid>> Create(ServerCreateModel objModel)
    {
        var obj = mapper.Map<Server>(objModel);
        
        await ValidateAndChangeModelStateAsync(validator, obj, CancellationToken.None);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        await crudProvider.Create(obj);
        
        return obj.Id;
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Guid>>> BulkCreate(List<ServerCreateModel> objModels)
    {
        var objects = mapper.Map<List<Server>>(objModels);
        
        foreach (var obj in objects)
            await ValidateAndChangeModelStateAsync(validator, obj, CancellationToken.None);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        await crudProvider.CreateRange(objects);
        return objects.Select(obj => obj.Id).ToList();
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Guid>> Update(ServerUpdateModel objModel)
    {
        var obj = mapper.Map<Server>(objModel);

        await ValidateAndChangeModelStateAsync(validator, obj, CancellationToken.None);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await crudProvider.Update(obj, 
            server => server.ApplicationName, 
            server => server.ServerAddress,
            server => server.ServerPortNumber,
            server => server.Active);

        return obj.Id;
    }
    
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Guid>> Delete(Guid id)
    {
        var result = await crudProvider.Delete(id);

        return result;
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<long>> Activate(Guid id)
    {
        var result =  await crudProvider.Activate(id);
        
        return result;
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<long>> Deactivate(Guid id)
    {
        var result = await crudProvider.Deactivate(id);
        
        return result;
    }
}