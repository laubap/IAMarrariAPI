using Microsoft.AspNetCore.Mvc;

// Controller que fornece um resumo de anomalias detectadas para o dashboard.
// Ele consulta o banco de dados e calcula métricas agregadas para um cliente.
[ApiController]
[Route("api/anomalias-dashboard")]
public class AnomaliaDashboardController : ControllerBase
{
    private readonly AppDbContext _context;

    public AnomaliaDashboardController(AppDbContext context)
    {
        _context = context;
    }

    // Endpoint: GET /api/anomalias-dashboard/resumo/{clienteId}
    // Retorna resumo de anomalias do cliente informado, incluindo totais,
    // média de risco e as tags mais problemáticas.
    [HttpGet("resumo/{clienteId}")]
    public IActionResult Resumo(string clienteId)
    {
        var anomalias = _context.AnomaliasDetectadas
            .Where(x => x.ClienteId == clienteId)
            .ToList();

        var total = anomalias.Count;

        var totalAnomalias = anomalias.Count(x => x.EhAnomalia);

        var totalCriticas = anomalias.Count(x =>
            x.ClassificacaoRisco == "critico" ||
            x.ClassificacaoRisco == "crítico");

        var mediaRisco = anomalias.Any()
            ? anomalias.Average(x => x.ScoreRiscoProcesso)
            : 0;

        var tagsMaisProblematicas = anomalias
            .Where(x => x.EhAnomalia)
            .GroupBy(x => x.TagName)
            .Select(g => new
            {
                tagName = g.Key,
                totalAnomalias = g.Count(),
                maiorRisco = g.Max(x => x.ScoreRiscoProcesso),
                mediaRisco = g.Average(x => x.ScoreRiscoProcesso)
            })
            .OrderByDescending(x => x.totalAnomalias)
            .ThenByDescending(x => x.maiorRisco)
            .Take(10)
            .ToList();

        return Ok(new
        {
            clienteId,
            totalRegistros = total,
            totalAnomalias,
            totalCriticas,
            mediaRisco,
            tagsMaisProblematicas
        });
    }
}