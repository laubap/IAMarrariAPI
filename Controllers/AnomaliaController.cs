using Microsoft.AspNetCore.Mvc;

// Controller que expõe APIs para detectar anomalias relacionadas a tags.
// Ele roteia as requisições para o serviço de predição e traduz a resposta
// em um status HTTP apropriado.
[ApiController]
[Route("api/anomalias")]
public class AnomaliaController : ControllerBase
{
    // Serviço injetado que contém a lógica de detecção de anomalias.
    private readonly PredictionService _predictionService;

    public AnomaliaController(PredictionService predictionService)
    {
        _predictionService = predictionService;
    }

    // Endpoint: POST /api/anomalias/tag
    // Recebe um corpo JSON com os dados da tag para análise de anomalia.
    // Se a predição ocorrer sem erros, retorna 200 OK com o resultado.
    // Se houver exceção, retorna 400 Bad Request com a mensagem de erro.
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