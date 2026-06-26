public class ProcessoHealthScoreService
{
    public ResultadoSaudeProcesso Calcular(List<AnaliseTagProcessoResultado> tags)
    {
        if (tags == null || !tags.Any())
        {
            return new ResultadoSaudeProcesso
            {
                ScoreSaude = 0,
                Classificacao = "sem_dados",
                Mensagem = "Não há tags suficientes para calcular a saúde do processo."
            };
        }

        double penalidadeTotal = 0;

        foreach (var tag in tags)
        {
            if (!tag.PerfilTreinado)
            {
                penalidadeTotal += 8;
                continue;
            }

            if (!tag.Encontrado)
            {
                penalidadeTotal += 8;
                continue;
            }

            if (!tag.EhAnomalia)
                continue;

            var pesoCriticidade = tag.Criticidade?.ToLower() switch
            {
                "critica" or "crítica" => 4.0,
                "alta" => 3.0,
                "media" or "média" => 2.0,
                "baixa" => 1.0,
                _ => 1.5
            };

            var pesoScore = tag.Score switch
            {
                >= 8 => 4.0,
                >= 6 => 3.0,
                >= 3 => 2.0,
                >= 2 => 1.0,
                _ => 0.5
            };

            penalidadeTotal += pesoCriticidade * pesoScore * 4;
        }

        var scoreSaude = 100 - (int)Math.Round(penalidadeTotal);

        if (scoreSaude < 0)
            scoreSaude = 0;

        var classificacao = scoreSaude switch
        {
            >= 85 => "saudavel",
            >= 65 => "atenção",
            >= 40 => "degradado",
            _ => "critico"
        };

        return new ResultadoSaudeProcesso
        {
            ScoreSaude = scoreSaude,
            Classificacao = classificacao,
            Mensagem = GerarMensagem(scoreSaude, classificacao)
        };
    }

    private string GerarMensagem(int scoreSaude, string classificacao)
    {
        return classificacao switch
        {
            "saudavel" => $"Processo saudável ({scoreSaude}/100). Nenhuma condição crítica relevante identificada.",
            "atenção" => $"Processo em atenção ({scoreSaude}/100). Existem sinais que devem ser acompanhados.",
            "degradado" => $"Processo degradado ({scoreSaude}/100). Recomenda-se investigação operacional.",
            "critico" => $"Processo crítico ({scoreSaude}/100). Recomenda-se ação prioritária.",
            _ => "Saúde do processo não classificada."
        };
    }
}

public class ResultadoSaudeProcesso
{
    public int ScoreSaude { get; set; }
    public string Classificacao { get; set; } = "";
    public string Mensagem { get; set; } = "";
}