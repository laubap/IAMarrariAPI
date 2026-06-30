using Microsoft.AspNetCore.Mvc;

// Controller que expõe a análise de inteligência de um processo.
// Ele delega a lógica de análise ao ProcessoInteligenciaService e
// retorna o resultado ou um erro HTTP em caso de falha.
[ApiController]
[Route("api/processos-inteligencia")]
public class ProcessoInteligenciaController : ControllerBase
{
    private readonly ProcessoInteligenciaService _processoInteligenciaService;

    public ProcessoInteligenciaController(ProcessoInteligenciaService processoInteligenciaService)
    {
        _processoInteligenciaService = processoInteligenciaService;
    }

    [HttpGet("{processoId}")]
    // Endpoint: GET /api/processos-inteligencia/{processoId}
    // Retorna a análise do processo informado pelo id.
    public async Task<IActionResult> AnalisarProcesso(int processoId)
    {
        try
        {
            var resultado = await _processoInteligenciaService.AnalisarProcesso(processoId);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                erro = ex.Message
            });
        }
    }
}