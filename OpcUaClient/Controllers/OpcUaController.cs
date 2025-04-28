using Microsoft.AspNetCore.Mvc;
using OpcUaClient.Domain.Models;
using OpcUaClient.Services.Interfaces;
using StatusCodes = Microsoft.AspNetCore.Http.StatusCodes;


namespace OpcUaClient.Controllers;

public class OpcUaController(IOpcUaService opcUaService) : BaseController
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> AddToMonitoring(string name, int nodeId)
    {
        opcUaService.AddMonitoringItem(new Tag(name, nodeId.ToString()));
        
        return 1;
    }
}