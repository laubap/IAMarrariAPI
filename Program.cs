using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers da API
builder.Services.AddControllers();

// Banco SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=ia_config.db"));

// Serviço que busca histórico real no Bridge
builder.Services.AddSingleton<IHistoricoService, BridgeHistoricoService>();

// Serviço que remove valores muito fora do padrão do histórico
builder.Services.AddScoped<HistoricoLimpezaService>();

// Serviço que treina/calcula o perfil estatístico de cada tag
builder.Services.AddScoped<PerfilTagService>();

// Serviço que detecta anomalia usando o perfil salvo da tag
builder.Services.AddScoped<PredictionService>();

//Serviço que identifica todas as tags do cliente
builder.Services.AddScoped<BridgeTagsService>();

//Tags relacionadas
builder.Services.AddScoped<AnaliseRelacionadasService>();

//Pega valor atual da tag
builder.Services.AddScoped<BridgeValorAtualService>();

//Ver historico de anomalias
builder.Services.AddScoped<AnomaliaHistoricoService>();

//Score de risco dos processos e tags
builder.Services.AddScoped<RiskScoreService>();

//Explicação do score de risco
builder.Services.AddScoped<ExplicacaoRiscoService>();

//Tendencia de risco dos processos e tags
builder.Services.AddScoped<TendenciaRiscoService>();

//Tendencia de valor das tags
builder.Services.AddScoped<TendenciaValorService>();

//Interpretação das dependências
builder.Services.AddScoped<InterpretacaoDependenciasService>();

// Serviço que busca processos e tags relacionadas
builder.Services.AddScoped<ProcessoAnaliseService>();

// Serviço que analisa processos e suas tags
builder.Services.AddScoped<ProcessoInteligenciaService>();

// Calcula o score de risco do processo
builder.Services.AddScoped<ProcessoHealthScoreService>();

// Interpreta o resultado da análise do processo
builder.Services.AddScoped<InterpretacaoProcessoService>();

// Serviço que interpreta um processo e gera uma probabilidade de falha
builder.Services.AddScoped<FailurePredictionService>();

// Serviço que analisa o equipamento e suas tags
builder.Services.AddScoped<EquipamentoInteligenciaService>();

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