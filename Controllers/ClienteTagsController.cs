using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/clientes")]
public class ClienteTagsController : ControllerBase
{
    private readonly BridgeTagsService _bridgeTagsService;

    public ClienteTagsController(
        BridgeTagsService bridgeTagsService)
    {
        _bridgeTagsService = bridgeTagsService;
    }

    [HttpGet("{clienteId}/tags")]
    public async Task<IActionResult> ListarTags(
        string clienteId)
    {
        var resultado =
            await _bridgeTagsService.BuscarTagsCliente(
                clienteId);

        return Content(
            resultado,
            "application/json");
    }
}