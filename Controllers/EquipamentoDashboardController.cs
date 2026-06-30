using Microsoft.AspNetCore.Mvc;

// Controller que fornece dados agregados para o dashboard de um equipamento.
// Ele combina análise de inteligência do equipamento com previsão de falhas
// para retornar um resumo completo de saúde e risco.
[ApiController]
[Route("api/equipamentos-dashboard")]
public class EquipamentoDashboardController : ControllerBase
{
    private readonly EquipamentoInteligenciaService _equipamentoInteligenciaService;
    private readonly FailurePredictionService _failurePredictionService;

    public EquipamentoDashboardController(
        EquipamentoInteligenciaService equipamentoInteligenciaService,
        FailurePredictionService failurePredictionService)
    {
        _equipamentoInteligenciaService = equipamentoInteligenciaService;
        _failurePredictionService = failurePredictionService;
    }

    [HttpGet("{equipamentoId}")]
    // Endpoint: GET /api/equipamentos-dashboard/{equipamentoId}
    // Retorna um modelo de dashboard com análise do equipamento, saúde,
    // métricas de tags, processos, previsão de falha e tags críticas.
    public async Task<IActionResult> Dashboard(int equipamentoId)
    {
        try
        {
            var analise = await _equipamentoInteligenciaService.AnalisarEquipamento(equipamentoId);

            var processoFake = new AnaliseProcessoResultado
            {
                ScoreSaude = analise.ScoreSaude,
                TotalTagsAnomalas = analise.TotalTagsAnomalas,
                TotalTagsSemPerfil = analise.TotalTagsSemPerfil,
                Tags = analise.Tags
            };

            var falha = _failurePredictionService.Calcular(processoFake);

            var dashboard = new
            {
                equipamentoId = analise.EquipamentoId,
                equipamento = analise.Nome,
                descricao = analise.Descricao,
                area = analise.Area,
                tipoEquipamento = analise.TipoEquipamento,
                criticidade = analise.Criticidade,
                fabricante = analise.Fabricante,
                modelo = analise.Modelo,
                dataUltimaManutencao = analise.DataUltimaManutencao,

                saude = analise.ScoreSaude,
                status = analise.ClassificacaoSaude,
                mensagemSaude = analise.MensagemSaude,

                totalTags = analise.TotalTags,
                tagsNormais = analise.Tags.Count(x => x.Status == "normal"),
                tagsAnomalas = analise.TotalTagsAnomalas,
                tagsSemPerfil = analise.TotalTagsSemPerfil,
                tagsSemValorAtual = analise.TotalTagsSemValorAtual,

                processos = analise.Processos,

                probabilidadeFalha = new
                {
                    percentual = falha.Probabilidade,
                    classificacao = falha.Classificacao,
                    mensagem = falha.Mensagem
                },

                interpretacao = analise.Interpretacao,

                tagsCriticas = analise.Tags
                    .Where(x =>
                        x.EhAnomalia ||
                        x.Status == "sem_perfil" ||
                        x.Status == "sem_valor_atual")
                    .Select(x => new
                    {
                        x.TagName,
                        x.Descricao,
                        x.PapelDaTag,
                        x.Criticidade,
                        x.Status,
                        x.Score,
                        x.Mensagem
                    })
                    .ToList()
            };

            return Ok(dashboard);
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