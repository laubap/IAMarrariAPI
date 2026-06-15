var builder = WebApplication.CreateBuilder(args);

// Cria a pasta onde os modelos treinados serão salvos
Directory.CreateDirectory("ModelsML");

// Treina os modelos por tipo de tag
TreinarModelo(
    entrada: "Data/temperatura.csv",
    enriquecido: "Data/temperatura_enriquecido.csv",
    modelo: "ModelsML/modelo_temperatura.zip"
);

TreinarModelo(
    entrada: "Data/pressao.csv",
    enriquecido: "Data/pressao_enriquecido.csv",
    modelo: "ModelsML/modelo_pressao.zip"
);

TreinarModelo(
    entrada: "Data/corrente.csv",
    enriquecido: "Data/corrente_enriquecido.csv",
    modelo: "ModelsML/modelo_corrente.zip"
);

// Controllers da API
builder.Services.AddControllers();

// Serviços usados pela API
builder.Services.AddSingleton<IHistoricoService, CsvHistoricoService>();
builder.Services.AddSingleton<PredictionService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

static void TreinarModelo(string entrada, string enriquecido, string modelo)
{
    Console.WriteLine();
    Console.WriteLine("====================================");
    Console.WriteLine($"Treinando modelo: {modelo}");
    Console.WriteLine($"CSV entrada: {entrada}");
    Console.WriteLine($"CSV enriquecido: {enriquecido}");

    if (!File.Exists(entrada))
        throw new Exception($"Arquivo não encontrado: {entrada}");

    var linhasEntrada = File.ReadAllLines(entrada)
        .Where(l => !string.IsNullOrWhiteSpace(l))
        .ToList();

    Console.WriteLine($"Linhas no CSV de entrada: {linhasEntrada.Count}");

    var detector = new AnomalyDetectionService();

    CsvFeatureGenerator.Gerar(
        caminhoEntrada: entrada,
        caminhoSaida: enriquecido
    );

    Console.WriteLine($"CSV enriquecido criado: {File.Exists(enriquecido)}");

    if (File.Exists(enriquecido))
    {
        var linhasEnriquecido = File.ReadAllLines(enriquecido)
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .ToList();

        Console.WriteLine($"Linhas no CSV enriquecido: {linhasEnriquecido.Count}");
    }

    detector.Treinar(enriquecido);
    detector.SalvarModelo(modelo);

    Console.WriteLine($"Modelo salvo com sucesso: {modelo}");
}