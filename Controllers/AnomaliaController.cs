using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/anomalias")]
public class AnomaliaController : ControllerBase
{
    private readonly PredictionService _predictionService;

    public AnomaliaController(PredictionService predictionService)
    {
        _predictionService = predictionService;
    }

    [HttpPost("tag")]
    public async Task<IActionResult> DetectarTag([FromBody] AnomaliaRequest request)
    {
        try
        {
            var resposta = await _predictionService.Detectar(request);
            return Ok(resposta);
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