// Serviço que analisa a tendência de risco para uma tag, com base nas últimas
// detecções de anomalia registradas para ela.
// Retorna se o risco está melhorando, piorando, estável ou se não há dados suficientes.
public class TendenciaRiscoService
{
    private readonly AppDbContext _context;

    public TendenciaRiscoService(AppDbContext context)
    {
        _context = context;
    }

    // Avalia a tendência do risco usando os últimos registros de anomalia da tag.
    public ResultadoTendenciaRisco Analisar(
        string clienteId,
        string tagName)
    {
        var historico = _context.AnomaliasDetectadas
            .Where(x =>
                x.ClienteId == clienteId &&
                x.TagName == tagName)
            .OrderByDescending(x => x.DataDeteccao)
            .Take(5)
            .OrderBy(x => x.DataDeteccao)
            .ToList();

        if (historico.Count < 2)
        {
            return new ResultadoTendenciaRisco
            {
                Tendencia = "insuficiente",
                Mensagem = "Ainda não existe histórico suficiente para análise de tendência."
            };
        }

        var primeiro = historico.First().ScoreRiscoProcesso;
        var ultimo = historico.Last().ScoreRiscoProcesso;

        var diferenca = ultimo - primeiro;

        if (diferenca >= 20)
        {
            return new ResultadoTendenciaRisco
            {
                Tendencia = "piora",
                Mensagem = $"O risco aumentou de {primeiro} para {ultimo} nas últimas análises."
            };
        }

        if (diferenca <= -20)
        {
            return new ResultadoTendenciaRisco
            {
                Tendencia = "melhora",
                Mensagem = $"O risco reduziu de {primeiro} para {ultimo} nas últimas análises."
            };
        }

        return new ResultadoTendenciaRisco
        {
            Tendencia = "estavel",
            Mensagem = $"O risco permaneceu próximo de {ultimo} nas últimas análises."
        };
    }
}

public class ResultadoTendenciaRisco
{
    public string Tendencia { get; set; } = "";
    public string Mensagem { get; set; } = "";
}