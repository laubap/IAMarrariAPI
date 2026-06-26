public class ExplicacaoRiscoService
{
    public List<string> GerarFatores(
        double score,
        TagContextoIa? contexto,
        AnaliseRelacionadasResultado analiseRelacionadas,
        ResultadoRiscoProcesso riscoProcesso)
    {
        var fatores = new List<string>();

        if (score >= 6)
            fatores.Add($"Score comportamental elevado ({score:F2}), indicando forte desvio do padrão histórico.");
        else if (score >= 3)
            fatores.Add($"Score comportamental moderado ({score:F2}), indicando desvio relevante do padrão histórico.");

        if (!string.IsNullOrWhiteSpace(contexto?.Criticidade))
            fatores.Add($"Tag classificada com criticidade {contexto.Criticidade}.");

        var dependenciasAltoImpacto = analiseRelacionadas.Resultados
            .Where(x =>
                string.Equals(x.Impacto, "alto", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(x.Impacto, "critico", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(x.Impacto, "crítico", StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (dependenciasAltoImpacto.Any())
        {
            var nomes = string.Join(", ", dependenciasAltoImpacto.Select(x => x.TagName));
            fatores.Add($"Existem dependências de alto impacto associadas: {nomes}.");
        }

        var dependenciasAnomalas = analiseRelacionadas.Resultados
            .Where(x => x.EhAnomalia)
            .ToList();

        if (dependenciasAnomalas.Any())
        {
            var nomes = string.Join(", ", dependenciasAnomalas.Select(x => x.TagName));
            fatores.Add($"Tags dependentes também apresentaram anomalia: {nomes}.");
        }

        if (!string.IsNullOrWhiteSpace(contexto?.Equipamento))
            fatores.Add($"Equipamento associado: {contexto.Equipamento}.");

        if (!string.IsNullOrWhiteSpace(contexto?.Area))
            fatores.Add($"Área do processo: {contexto.Area}.");

        fatores.Add($"Classificação final do risco: {riscoProcesso.Classificacao} ({riscoProcesso.ScoreRisco}/100).");

        return fatores;
    }
}