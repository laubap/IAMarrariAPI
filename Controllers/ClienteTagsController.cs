using Microsoft.AspNetCore.Mvc;

// Controller que expõe endpoint para listar tags de um cliente.
// Ele delega a busca de tags ao serviço BridgeTagsService.
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
    // Endpoint: GET /api/clientes/{clienteId}/tags
    // Retorna a lista de tags do cliente em JSON usando o serviço de ponte.
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