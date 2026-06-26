public class InterpretacaoDependenciasService
{
    public string Interpretar(ResultadoTagRelacionada item, TagContextoIa? contextoPrincipal)
    {
        var partes = new List<string>();

        partes.Add(InterpretarRelacao(item));
        partes.Add(InterpretarMesmoEquipamento(item, contextoPrincipal));
        partes.Add(InterpretarMesmaArea(item, contextoPrincipal));
        partes.Add(InterpretarCriticidade(item));
        partes.Add(InterpretarImpacto(item));
        partes.Add(GerarConclusao(item, contextoPrincipal));

        return string.Join(" ", partes.Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    private string InterpretarRelacao(ResultadoTagRelacionada item)
    {
        var tipo = item.TipoRelacao?.ToLower();

        return tipo switch
        {
            "depende_da_principal" => item.EhAnomalia
                ? "A tag dependente também apresentou anomalia, indicando possível propagação do problema."
                : "A tag dependente permanece normal, sugerindo que o efeito ainda pode estar restrito à tag principal.",

            "confirma_anomalia" => item.EhAnomalia
                ? "A tag dependente confirma a anomalia observada na tag principal, aumentando a confiança da detecção."
                : "A tag dependente não confirmou a anomalia, reduzindo a confiança de que o problema esteja espalhado no processo.",

            "indicador_do_mesmo_processo" => item.EhAnomalia
                ? "A tag dependente monitora o mesmo processo e também está anômala, sugerindo impacto mais amplo."
                : "A tag dependente monitora o mesmo processo e permanece estável, sugerindo alteração localizada.",

            _ => item.EhAnomalia
                ? "A tag dependente também apresentou comportamento anormal."
                : "A tag dependente permanece dentro do comportamento esperado."
        };
    }

    private string InterpretarMesmoEquipamento(ResultadoTagRelacionada item, TagContextoIa? contextoPrincipal)
    {
        if (contextoPrincipal == null)
            return "";

        if (string.IsNullOrWhiteSpace(contextoPrincipal.Equipamento) ||
            string.IsNullOrWhiteSpace(item.Equipamento))
            return "";

        if (string.Equals(contextoPrincipal.Equipamento, item.Equipamento, StringComparison.OrdinalIgnoreCase))
        {
            return item.EhAnomalia
                ? $"As duas tags estão associadas ao mesmo equipamento ({item.Equipamento}), reforçando a possibilidade de problema real nesse equipamento."
                : $"As duas tags estão associadas ao mesmo equipamento ({item.Equipamento}), mas a dependente permanece normal, o que pode indicar falha localizada na variável principal.";
        }

        return "";
    }

    private string InterpretarMesmaArea(ResultadoTagRelacionada item, TagContextoIa? contextoPrincipal)
    {
        if (contextoPrincipal == null)
            return "";

        if (string.IsNullOrWhiteSpace(contextoPrincipal.Area) ||
            string.IsNullOrWhiteSpace(item.Area))
            return "";

        if (string.Equals(contextoPrincipal.Area, item.Area, StringComparison.OrdinalIgnoreCase))
        {
            return item.EhAnomalia
                ? $"As tags pertencem à mesma área operacional ({item.Area}), indicando possível impacto no processo local."
                : $"As tags pertencem à mesma área operacional ({item.Area}), porém a dependente está normal, sugerindo que a anomalia ainda não se espalhou pela área.";
        }

        return "";
    }

    private string InterpretarCriticidade(ResultadoTagRelacionada item)
    {
        var criticidade = item.Criticidade?.ToLower();

        if (criticidade == "alta" || criticidade == "critica" || criticidade == "crítica")
            return "A tag dependente possui criticidade elevada, portanto qualquer alteração futura pode aumentar rapidamente o risco operacional.";

        return "";
    }

    private string InterpretarImpacto(ResultadoTagRelacionada item)
    {
        var impacto = item.Impacto?.ToLower();

        if (impacto == "alto" || impacto == "critico" || impacto == "crítico")
            return "A relação cadastrada possui impacto alto, então essa dependência deve ser priorizada no monitoramento.";

        if (impacto == "medio" || impacto == "médio")
            return "A relação cadastrada possui impacto médio, recomendando acompanhamento da condição.";

        return "";
    }

    private string GerarConclusao(ResultadoTagRelacionada item, TagContextoIa? contextoPrincipal)
    {
        var descPrincipal = contextoPrincipal?.Descricao;
        var descDependente = item.Descricao;

        if (!string.IsNullOrWhiteSpace(descPrincipal) &&
            !string.IsNullOrWhiteSpace(descDependente))
        {
            if (item.EhAnomalia)
            {
                return $"Conclusão: a anomalia em {descPrincipal} junto com alteração em {descDependente} reforça a hipótese de impacto no processo.";
            }

            return $"Conclusão: a anomalia em {descPrincipal} não foi acompanhada por alteração em {descDependente}, reduzindo a evidência de propagação imediata.";
        }

        return item.EhAnomalia
            ? "Conclusão: a dependente também está anômala, aumentando a prioridade da investigação."
            : "Conclusão: a dependente está normal, indicando que o problema pode estar restrito à tag principal.";
    }
}