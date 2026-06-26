using Microsoft.AspNetCore.Mvc;

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