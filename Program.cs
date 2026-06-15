var builder = WebApplication.CreateBuilder(args);

// Cria a pasta dos modelos caso não exista
Directory.CreateDirectory("ModelsML");

// Treina um novo modelo usando o CSV atual
var detectorTreino = new AnomalyDetectionService();

CsvFeatureGenerator.Gerar(
    caminhoEntrada: "Data/sensores.csv",
    caminhoSaida: "Data/sensores_enriquecido.csv"
);

detectorTreino.Treinar("Data/sensores_enriquecido.csv");
detectorTreino.SalvarModelo("ModelsML/modelo_tag.zip");

// Controllers da API
builder.Services.AddControllers();

// Serviços
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