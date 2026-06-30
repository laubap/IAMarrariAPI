// Serviço que calcula a probabilidade de falha com base no resultado da análise de processo.
// Ele transforma o score de saúde, contagem de tags anômalas e tags sem perfil em uma
// probabilidade percentual e classificação de risco.
public class FailurePredictionService
{
    public ResultadoProbabilidadeFalha Calcular(AnaliseProcessoResultado processo)
    {
        var probabilidade = 100 - processo.ScoreSaude;

        if (processo.TotalTagsAnomalas > 0)
            probabilidade += processo.TotalTagsAnomalas * 8;

        if (processo.TotalTagsSemPerfil > 0)
            probabilidade += processo.TotalTagsSemPerfil * 3;

        var anomalasCriticas = processo.Tags.Count(x =>
            x.EhAnomalia &&
            (
                x.Criticidade?.ToLower() == "alta" ||
                x.Criticidade?.ToLower() == "critica" ||
                x.Criticidade?.ToLower() == "crítica"
            ));

        probabilidade += anomalasCriticas * 12;

        if (probabilidade > 100)
            probabilidade = 100;

        var classificacao = probabilidade switch
        {
            >= 80 => "critica",
            >= 60 => "alta",
            >= 30 => "moderada",
            >= 1 => "baixa",
            _ => "muito_baixa"
        };

        return new ResultadoProbabilidadeFalha
        {
            Probabilidade = probabilidade,
            Classificacao = classificacao,
            Mensagem = classificacao switch
            {
                "critica" => "Probabilidade crítica de falha. Recomenda-se ação imediata.",
                "alta" => "Probabilidade alta de falha. Recomenda-se investigação prioritária.",
                "moderada" => "Probabilidade moderada de falha. Recomenda-se acompanhamento.",
                "baixa" => "Probabilidade baixa de falha no momento.",
                _ => "Probabilidade muito baixa de falha no momento."
            }
        };
    }
}

// Modelo de retorno para a probabilidade de falha calculada.
public class ResultadoProbabilidadeFalha
{
    public int Probabilidade { get; set; }
    public string Classificacao { get; set; } = "";
    public string Mensagem { get; set; } = "";
}