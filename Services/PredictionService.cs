public class PredictionService
{
    private readonly AnomalyDetectionService _detector;
    private readonly IHistoricoService _historicoService;

    public PredictionService(IHistoricoService historicoService)
    {
        _historicoService = historicoService;

        _detector = new AnomalyDetectionService();

        // Carrega o modelo treinado salvo em ZIP
        _detector.CarregarModelo("ModelsML/modelo_tag.zip");
    }

    public AnomaliaResponse Detectar(AnomaliaRequest request)
    {
        // Busca o histórico da tag.
        // Hoje ele busca em CSV local.
        // Futuramente isso será trocado pela biblioteca do Viewer.
        var historico = _historicoService.BuscarHistorico(request.TagName);

        // Calcula as features com base no histórico + nova leitura
        var entrada = CsvFeatureGenerator.GerarFeaturesParaNovaLeitura(
            historico,
            request.DataHora,
            request.Valor
        );

        var resultado = _detector.Prever(entrada);

        return new AnomaliaResponse
        {
            ClienteId = request.ClienteId,
            TagName = request.TagName,
            EhAnomalia = resultado.EhAnomalia,
            Score = resultado.Score,
            Mensagem = resultado.EhAnomalia
                ? $"Anomalia detectada na tag {request.TagName}."
                : $"Comportamento normal na tag {request.TagName}."
        };
    }
}