using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpcUaClient.Domain.Interfaces.Providers;
using OpcUaClient.Domain.Models;
using OpcUaClient.Models.TagModels;

namespace OpcUaClient.Controllers;

public class TagController(ITagCrudProvider crudProvider, IValidator<Tag> validator, IMapper mapper, IConfiguration configuration) : BaseEntityController<Tag>
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
            tag => tag.Comment,
            tag => tag.Active,
            tag => tag.Address,
            tag => tag.Type,
            tag => tag.Factor,
            tag => tag.Offset,
            tag => tag.Recalc,
            //tag => tag.Value,
            tag => tag.HasWriteRegister,
            tag => tag.WriteRegisterAddress,
            tag => tag.OutputType,
            tag => tag.UseOutputType,
            tag => tag.RoundingAccuracy,
            tag => tag.ConvertBeforeRecalcForRead,
            tag => tag.ConvertBeforeRecalcForWrite);
        
        return obj.Id;
    }
    
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Guid>> Delete(Guid id)
    {
        return await crudProvider.Delete(id);
    }
}