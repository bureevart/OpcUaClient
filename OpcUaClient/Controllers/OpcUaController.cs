using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using OpcUaClient.Model;
using OpcUaClient.Services.Interfaces;
using StatusCodes = Microsoft.AspNetCore.Http.StatusCodes;


namespace OpcUaClient.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class OpcUaController
{

    private readonly IOpcUaService OpcUaService;
    
    public OpcUaController(IOpcUaService opcUaService)
    {
        OpcUaService = opcUaService;
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> AddToMonitoring(string name, int nodeId)
    {
        OpcUaService.AddMonitoringItem(new TagClass(name, nodeId.ToString()));
        
        return 1;
    }
}