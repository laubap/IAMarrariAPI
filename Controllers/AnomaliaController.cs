using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/anomalias")]

//porta de entrada para os dados , toda detecção que chega para detectar anomalias passa por aq

public class AnomaliaController : ControllerBase
{
    private readonly PredictionService _predictionService;

    public AnomaliaController(PredictionService predictionService)
    {
        _predictionService = predictionService;
    }

    [HttpPost("tag")]
    public IActionResult DetectarTag([FromBody] AnomaliaRequest request)
    {
         // Envia os dados recebidos para a IA
        var resposta = _predictionService.Detectar(request);

        return Ok(resposta);
    }
}