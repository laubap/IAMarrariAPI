public class PredictionService
{
    private readonly IHistoricoService _historicoService;
    private readonly AppDbContext _context;

    public PredictionService(
        IHistoricoService historicoService,
        AppDbContext context
    )
    {
        _historicoService = historicoService;
        _context = context;
    }

    public AnomaliaResponse Detectar(AnomaliaRequest request)
    {
        var config = _context.TagsIaConfig
            .FirstOrDefault(x =>
                x.ClienteId == request.ClienteId &&
                x.TagName == request.TagName &&
                x.IaAtiva);

        if (config == null)
            throw new Exception($"IA não configurada para a tag {request.TagName} do cliente {request.ClienteId}.");

        var tipoTag = config.TipoTag.ToLower();

        var caminhoModelo = tipoTag switch
        {
            "temperatura" => "ModelsML/modelo_temperatura.zip",
            "pressao" => "ModelsML/modelo_pressao.zip",
            "corrente" => "ModelsML/modelo_corrente.zip",
            _ => throw new Exception($"Tipo de tag inválido: {tipoTag}")
        };

        var detector = new AnomalyDetectionService();
        detector.CarregarModelo(caminhoModelo);

        var historico = _historicoService.BuscarHistorico(
            request.TagName,
            tipoTag
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
            TipoTag = tipoTag,
            EhAnomalia = resultado.EhAnomalia,
            Score = resultado.Score,
            Mensagem = resultado.EhAnomalia
                ? $"Anomalia detectada na tag {request.TagName}."
                : $"Comportamento normal na tag {request.TagName}."
        };
    }
}