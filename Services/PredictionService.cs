public class PredictionService
{
    private readonly IHistoricoService _historicoService;

    public PredictionService(IHistoricoService historicoService)
    {
        _historicoService = historicoService;
    }

    public AnomaliaResponse Detectar(AnomaliaRequest request)
    {    //aqui é onde é escolhido o modelo de ML com base no tipo de TAG, oq acontece é como nao temos ainda o tipo de tag no server o proprio cliente tera q informar q tipo de tag é 
        var caminhoModelo = request.TipoTag.ToLower() switch
        {
            "temperatura" => "ModelsML/modelo_temperatura.zip",
            "pressao" => "ModelsML/modelo_pressao.zip",
            "corrente" => "ModelsML/modelo_corrente.zip",
            _ => throw new Exception($"Tipo de tag inválido: {request.TipoTag}")
        };

        var detector = new AnomalyDetectionService();
        detector.CarregarModelo(caminhoModelo);

        var historico = _historicoService.BuscarHistorico(
            request.TagName,
            request.TipoTag
        );

        var entrada = CsvFeatureGenerator.GerarFeaturesParaNovaLeitura(
            historico,
            request.DataHora,
            request.Valor
        );

        var resultado = detector.Prever(entrada);

        return new AnomaliaResponse
        {
            ClienteId = request.ClienteId,
            TagName = request.TagName,
            TipoTag = request.TipoTag,
            EhAnomalia = resultado.EhAnomalia,
            Score = resultado.Score,
            Mensagem = resultado.EhAnomalia
                ? $"Anomalia detectada na tag {request.TagName}."
                : $"Comportamento normal na tag {request.TagName}."
        };
    }
}