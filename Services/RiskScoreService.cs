public class RiskScoreService
{
    public ResultadoRiscoProcesso Calcular(
        bool ehAnomalia,
        double score,
        string? criticidade,
        AnaliseRelacionadasResultado analiseRelacionadas)
    {
        var risco = 0;

        if (ehAnomalia)
            risco += 35;

        if (score >= 6)
            risco += 25;
        else if (score >= 3)
            risco += 15;
        else if (score >= 2)
            risco += 5;

        risco += criticidade?.ToLower() switch
        {
            "critica" or "crítica" => 25,
            "alta" => 20,
            "media" or "média" => 10,
            "baixa" => 5,
            _ => 0
        };

        risco += analiseRelacionadas.TotalRelacionadasAnomalas * 15;

        var totalImpactoAlto = analiseRelacionadas.Resultados.Count(x =>
            string.Equals(x.Impacto, "alto", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(x.Impacto, "critico", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(x.Impacto, "crítico", StringComparison.OrdinalIgnoreCase));

        risco += totalImpactoAlto * 10;

        if (risco > 100)
            risco = 100;

        var classificacao = risco switch
        {
            >= 80 => "critico",
            >= 60 => "alto",
            >= 35 => "medio",
            >= 1 => "baixo",
            _ => "normal"
        };

        return new ResultadoRiscoProcesso
        {
            ScoreRisco = risco,
            Classificacao = classificacao,
            Mensagem = classificacao switch
            {
                "critico" => "Risco crítico para o processo. Recomenda-se ação imediata.",
                "alto" => "Risco alto para o processo. Recomenda-se investigação prioritária.",
                "medio" => "Risco médio. Recomenda-se acompanhamento da condição.",
                "baixo" => "Risco baixo. Acompanhar caso o comportamento se repita.",
                _ => "Processo dentro do comportamento esperado."
            }
        };
    }
}

public class ResultadoRiscoProcesso
{
    public int ScoreRisco { get; set; }
    public string Classificacao { get; set; } = "";
    public string Mensagem { get; set; } = "";
}