public class InterpretacaoProcessoService
{
    public string Interpretar(AnaliseProcessoResultado processo)
    {
        var partes = new List<string>();

        partes.Add(InterpretarSaude(processo));
        partes.Add(InterpretarAnomalias(processo));
        partes.Add(InterpretarTagsSemPerfil(processo));
        partes.Add(InterpretarCriticidade(processo));
        partes.Add(GerarConclusao(processo));

        return string.Join(" ", partes.Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    private string InterpretarSaude(AnaliseProcessoResultado processo)
    {
        return processo.ClassificacaoSaude switch
        {
            "saudavel" => $"O processo {processo.NomeProcesso} está saudável, com índice de saúde {processo.ScoreSaude}/100.",
            "atenção" => $"O processo {processo.NomeProcesso} exige atenção, com índice de saúde {processo.ScoreSaude}/100.",
            "degradado" => $"O processo {processo.NomeProcesso} apresenta degradação operacional, com índice de saúde {processo.ScoreSaude}/100.",
            "critico" => $"O processo {processo.NomeProcesso} está em condição crítica, com índice de saúde {processo.ScoreSaude}/100.",
            _ => $"O processo {processo.NomeProcesso} possui saúde {processo.ScoreSaude}/100."
        };
    }

    private string InterpretarAnomalias(AnaliseProcessoResultado processo)
    {
        if (processo.TotalTagsAnomalas == 0)
            return "Nenhuma tag do processo apresentou comportamento anormal no momento.";

        var anomalas = processo.Tags
            .Where(x => x.EhAnomalia)
            .Select(x => !string.IsNullOrWhiteSpace(x.Descricao)
                ? $"{x.TagName} ({x.Descricao})"
                : x.TagName)
            .ToList();

        return $"Foram identificadas {processo.TotalTagsAnomalas} tag(s) anômalas no processo: {string.Join(", ", anomalas)}.";
    }

    private string InterpretarTagsSemPerfil(AnaliseProcessoResultado processo)
    {
        if (processo.TotalTagsSemPerfil == 0)
            return "Todas as tags do processo possuem perfil treinado.";

        return $"{processo.TotalTagsSemPerfil} tag(s) ainda não possuem perfil treinado, então a análise do processo ainda não está completa.";
    }

    private string InterpretarCriticidade(AnaliseProcessoResultado processo)
    {
        var criticidade = processo.Criticidade?.ToLower();

        if (criticidade == "alta" || criticidade == "critica" || criticidade == "crítica")
            return "Como o processo possui criticidade elevada, desvios persistentes devem ser priorizados pela operação.";

        if (criticidade == "media" || criticidade == "média")
            return "O processo possui criticidade média e deve ser acompanhado caso novas anomalias apareçam.";

        return "";
    }

    private string GerarConclusao(AnaliseProcessoResultado processo)
    {
        if (processo.TotalTagsAnomalas == 0 && processo.TotalTagsSemPerfil == 0)
            return "Conclusão: o processo aparenta estar operacionalmente estável.";

        if (processo.TotalTagsAnomalas == 0 && processo.TotalTagsSemPerfil > 0)
            return "Conclusão: não há anomalias detectadas, mas ainda existem tags sem perfil treinado, reduzindo a confiabilidade da análise.";

        if (processo.TotalTagsAnomalas == 1)
            return "Conclusão: há uma anomalia isolada. Recomenda-se verificar a tag afetada antes de assumir falha sistêmica no processo.";

        return "Conclusão: múltiplas tags apresentaram anomalia, indicando possível degradação do processo e necessidade de investigação prioritária.";
    }
}