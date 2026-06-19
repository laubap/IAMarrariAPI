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