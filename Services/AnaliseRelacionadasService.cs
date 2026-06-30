// Serviço responsável por analisar tags relacionadas a uma tag principal.
// Ele consulta dependências cadastradas, obtém valores atuais das tags dependentes
// e compara com seus perfis para detectar anomalias e impactar o resumo.
public class AnaliseRelacionadasService
{
    private readonly AppDbContext _context;
    private readonly BridgeValorAtualService _bridgeValorAtualService;

    public AnaliseRelacionadasService(
        AppDbContext context,
        BridgeValorAtualService bridgeValorAtualService)
    {
        _context = context;
        _bridgeValorAtualService = bridgeValorAtualService;
    }

    // Analisa as tags relacionadas à tag principal informada.
    // Retorna um resumo com o total de dependências, anomalias detectadas,
    // dependências de alto impacto e resultados detalhados por tag.
    public async Task<AnaliseRelacionadasResultado> Analisar(
        string clienteId,
        string tagName)
    {
        var dependencias = _context.TagDependenciasIa
            .Where(x =>
                x.ClienteId == clienteId &&
                x.TagName == tagName)
            .ToList();

        if (!dependencias.Any())
        {
            return new AnaliseRelacionadasResultado
            {
                Resumo = "Nenhuma dependência cadastrada para análise de processo."
            };
        }

        var tagsDependentes = dependencias
            .Select(x => x.TagDependente)
            .Distinct()
            .ToList();

        var valoresAtuais =
            await _bridgeValorAtualService.BuscarValoresAtuais(tagsDependentes);

        var resultados = new List<ResultadoTagRelacionada>();

        foreach (var dependencia in dependencias)
        {
            var tagDependente = dependencia.TagDependente;

            var contextoDependente = _context.TagsContextoIa
                .FirstOrDefault(x =>
                    x.ClienteId == clienteId &&
                    x.TagName == tagDependente);

            var perfil = _context.PerfisIa
                .FirstOrDefault(x =>
                    x.ClienteId == clienteId &&
                    x.TagName == tagDependente);

            var valorAtual = valoresAtuais
                .FirstOrDefault(x =>
                    string.Equals(
                        x.TagName,
                        tagDependente,
                        StringComparison.OrdinalIgnoreCase));

            if (perfil == null)
            {
                resultados.Add(CriarResultadoBase(
                    tagDependente,
                    dependencia,
                    contextoDependente,
                    valorAtual?.Encontrado ?? false,
                    false,
                    "Tag dependente ainda não possui perfil treinado."
                ));

                continue;
            }

            if (valorAtual == null || !valorAtual.Encontrado)
            {
                var resultado = CriarResultadoBase(
                    tagDependente,
                    dependencia,
                    contextoDependente,
                    false,
                    true,
                    "Valor atual da tag dependente não encontrado no Bridge."
                );

                resultado.Media = perfil.Media;
                resultado.DesvioPadrao = perfil.DesvioPadrao;

                resultados.Add(resultado);
                continue;
            }

            var limiteInferior = perfil.Media - (3 * perfil.DesvioPadrao);
            var limiteSuperior = perfil.Media + (3 * perfil.DesvioPadrao);

            var ehAnomalia =
                valorAtual.Valor < limiteInferior ||
                valorAtual.Valor > limiteSuperior;

            var nomeExibicao = !string.IsNullOrWhiteSpace(contextoDependente?.Descricao)
                ? $"{tagDependente} ({contextoDependente.Descricao})"
                : tagDependente;

            var mensagem = ehAnomalia
                ? $"Tag dependente {nomeExibicao} também está fora do perfil esperado. Impacto informado: {dependencia.Impacto}."
                : $"Tag dependente {nomeExibicao} está dentro do perfil esperado. Impacto informado: {dependencia.Impacto}.";

            if (contextoDependente != null)
            {
                if (!string.IsNullOrWhiteSpace(contextoDependente.Criticidade))
                    mensagem += $" Criticidade da dependente: {contextoDependente.Criticidade}.";

                if (!string.IsNullOrWhiteSpace(contextoDependente.Equipamento))
                    mensagem += $" Equipamento da dependente: {contextoDependente.Equipamento}.";

                if (!string.IsNullOrWhiteSpace(contextoDependente.Area))
                    mensagem += $" Área da dependente: {contextoDependente.Area}.";
            }

            var resultadoFinal = CriarResultadoBase(
                tagDependente,
                dependencia,
                contextoDependente,
                true,
                true,
                mensagem
            );

            resultadoFinal.ValorAtual = valorAtual.Valor;
            resultadoFinal.DataHora = valorAtual.DataHora;
            resultadoFinal.Media = perfil.Media;
            resultadoFinal.DesvioPadrao = perfil.DesvioPadrao;
            resultadoFinal.LimiteInferior = limiteInferior;
            resultadoFinal.LimiteSuperior = limiteSuperior;
            resultadoFinal.EhAnomalia = ehAnomalia;

            resultados.Add(resultadoFinal);
        }

        var qtdAnomalias = resultados.Count(x => x.EhAnomalia);
        var qtdImpactoAlto = resultados.Count(x =>
            string.Equals(x.Impacto, "alto", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(x.Impacto, "critico", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(x.Impacto, "crítico", StringComparison.OrdinalIgnoreCase));

        var resumo = MontarResumo(qtdAnomalias, qtdImpactoAlto, resultados);

        return new AnaliseRelacionadasResultado
        {
            TotalTagsRelacionadas = resultados.Count,
            TotalRelacionadasAnomalas = qtdAnomalias,
            TotalDependenciasImpactoAlto = qtdImpactoAlto,
            Resultados = resultados,
            Resumo = resumo
        };
    }

    // Cria o objeto base de resultado para uma tag dependente,
    // preenchendo metadados de relação e contexto.
    private ResultadoTagRelacionada CriarResultadoBase(
        string tagDependente,
        TagDependenciaIa dependencia,
        TagContextoIa? contextoDependente,
        bool encontrado,
        bool perfilTreinado,
        string mensagem)
    {
        return new ResultadoTagRelacionada
        {
            TagName = tagDependente,

            TipoRelacao = dependencia.TipoRelacao,
            Impacto = dependencia.Impacto,
            DescricaoRelacao = dependencia.Descricao,

            Encontrado = encontrado,
            PerfilTreinado = perfilTreinado,
            Mensagem = mensagem,

            PossuiContexto = contextoDependente != null,
            Descricao = contextoDependente?.Descricao,
            Criticidade = contextoDependente?.Criticidade,
            Equipamento = contextoDependente?.Equipamento,
            Area = contextoDependente?.Area,
            ImpactosContexto = contextoDependente?.Impactos,
            AcoesRecomendadasContexto = contextoDependente?.AcoesRecomendadas
        };
    }

    // Monta uma mensagem de resumo baseada na quantidade de anomalias e impacto alto.
    private string MontarResumo(
        int qtdAnomalias,
        int qtdImpactoAlto,
        List<ResultadoTagRelacionada> resultados)
    {
        if (!resultados.Any())
            return "Nenhuma dependência foi analisada.";

        if (qtdAnomalias == 0 && qtdImpactoAlto == 0)
            return "Nenhuma tag dependente apresentou comportamento anormal e não há dependências de alto impacto cadastradas.";

        if (qtdAnomalias == 0 && qtdImpactoAlto > 0)
            return "Nenhuma tag dependente apresentou comportamento anormal, porém existem dependências de alto impacto associadas à tag principal.";

        if (qtdAnomalias > 0 && qtdImpactoAlto == 0)
            return $"{qtdAnomalias} tag(s) dependente(s) também apresentam comportamento anormal.";

        return $"{qtdAnomalias} tag(s) dependente(s) também apresentam comportamento anormal e existem dependências de alto impacto associadas à tag principal.";
    }
}

public class AnaliseRelacionadasResultado
{
    public int TotalTagsRelacionadas { get; set; }
    public int TotalRelacionadasAnomalas { get; set; }
    public int TotalDependenciasImpactoAlto { get; set; }

    public string Resumo { get; set; } = "";

    public List<ResultadoTagRelacionada> Resultados { get; set; } = new();
}

public class ResultadoTagRelacionada
{
    public string TagName { get; set; } = "";

    public string? TipoRelacao { get; set; }
    public string? Impacto { get; set; }
    public string? DescricaoRelacao { get; set; }

    public double ValorAtual { get; set; }
    public DateTime DataHora { get; set; }

    public double Media { get; set; }
    public double DesvioPadrao { get; set; }
    public double LimiteInferior { get; set; }
    public double LimiteSuperior { get; set; }

    public bool EhAnomalia { get; set; }
    public bool PerfilTreinado { get; set; }
    public bool Encontrado { get; set; }

    public bool PossuiContexto { get; set; }
    public string? Descricao { get; set; }
    public string? Criticidade { get; set; }
    public string? Equipamento { get; set; }
    public string? Area { get; set; }
    public string? ImpactosContexto { get; set; }
    public string? AcoesRecomendadasContexto { get; set; }

    public string Mensagem { get; set; } = "";
}