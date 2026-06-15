using Microsoft.ML;

public class AnomalyDetectionService
{
    private readonly MLContext _mlContext = new();

    private PredictionEngine<SensorData, SensorPrediction>? _predictor;
    private ITransformer? _modelo;
    private DataViewSchema? _schema;

    public void Treinar(string caminhoCsvEnriquecido)
    {
        var dataView = _mlContext.Data.LoadFromTextFile<SensorData>(
            path: caminhoCsvEnriquecido,
            hasHeader: true,
            separatorChar: ';'
        );

        var pipeline = _mlContext.Transforms.Concatenate(
                "Features",
                nameof(SensorData.ValorAtual),
                nameof(SensorData.MediaMovel5),
                nameof(SensorData.Variacao),
                nameof(SensorData.MinJanela5),
                nameof(SensorData.MaxJanela5),
                nameof(SensorData.DesvioPadrao5)
            )
            .Append(_mlContext.AnomalyDetection.Trainers.RandomizedPca(
                featureColumnName: "Features",
                rank: 3
            ));

        var modelo = pipeline.Fit(dataView);

        _modelo = modelo;
        _schema = dataView.Schema;

        _predictor = _mlContext.Model.CreatePredictionEngine<SensorData, SensorPrediction>(modelo);

        Console.WriteLine("Modelo treinado com sucesso!");
    }

    public void SalvarModelo(string caminho)
    {
        if (_modelo == null || _schema == null)
            throw new InvalidOperationException("Modelo ainda não foi treinado.");

        _mlContext.Model.Save(_modelo, _schema, caminho);

        Console.WriteLine($"Modelo salvo em: {caminho}");
    }

    public void CarregarModelo(string caminhoModelo)
    {
        var modelo = _mlContext.Model.Load(caminhoModelo, out var schema);

        _modelo = modelo;
        _schema = schema;

        _predictor = _mlContext.Model.CreatePredictionEngine<SensorData, SensorPrediction>(modelo);

        Console.WriteLine($"Modelo carregado de: {caminhoModelo}");
    }

    public SensorPrediction PreverNovaLeitura(
        string caminhoHistorico,
        DateTime dataHora,
        float valor
    )
    {
        if (_predictor == null)
            throw new InvalidOperationException("O modelo ainda não foi treinado.");

        var historico = CsvFeatureGenerator.CarregarHistorico(caminhoHistorico);

        var entrada = CsvFeatureGenerator.GerarFeaturesParaNovaLeitura(
            historico,
            dataHora,
            valor
        );

        return _predictor.Predict(entrada);
    }

    public SensorPrediction Prever(SensorData entrada)
    {
        if (_predictor == null)
            throw new InvalidOperationException("O modelo ainda não foi carregado.");

        return _predictor.Predict(entrada);
    }
}