using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OpcUaClient.Domain.Interfaces.Providers;
using OpcUaClient.Domain.Models;
using OpcUaClient.Models.TagModels;
using OpcUaClient.Services.Interfaces;

namespace OpcUaClient.Controllers;

public class TagController(ITagCrudProvider crudProvider, IValidator<Tag> validator, IMapper mapper, IOpcUaService opcUaService) : BaseEntityController<Tag>
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TagViewModel>>> GetAll()
    {
        var result = await crudProvider.GetAll();
        return mapper.Map<List<TagViewModel>>(result);
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TagViewModel>> Get(Guid id)
    {
        var result = await crudProvider.Get(id);
        return mapper.Map<TagViewModel>(result);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Guid>> Create(TagCreateModel objModel)
    {
        var obj = mapper.Map<Tag>(objModel);
        
        await ValidateAndChangeModelStateAsync(validator, obj, CancellationToken.None);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        await crudProvider.Create(obj);
        
        opcUaService.AddMonitoringItem(obj);
        return obj.Id;
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Guid>>> BulkCreate(List<TagCreateModel> objModels)
    {
        var objects = mapper.Map<List<Tag>>(objModels);
        
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
    public async Task<ActionResult<Guid>> Update(TagUpdateModel objModel)
    {
        var obj = mapper.Map<Tag>(objModel);
        
        await ValidateAndChangeModelStateAsync(validator, obj, CancellationToken.None);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        await crudProvider.Update(obj, 
            tag => tag.Name,
            tag => tag.LastUpdatedTime,
            tag => tag.LastSourceTimeStamp,
            tag => tag.StatusCode,
            tag => tag.LastGoodValue ?? string.Empty,
            tag => tag.CurrentValue ?? string.Empty,
            tag => tag.NodeId,
            tag => tag.DisplayName);
        
        return obj.Id;
    }
    
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Guid>> Delete(Guid id)
    {
        return await crudProvider.Delete(id);
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TagShortViewModel>> GetValue(Guid id)
    {
        var result = await crudProvider.GetValue(id);
        return mapper.Map<TagShortViewModel>(result);
    }
}