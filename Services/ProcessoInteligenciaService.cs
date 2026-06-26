using Microsoft.EntityFrameworkCore;

public class ProcessoInteligenciaService
{
    private readonly AppDbContext _context;
    private readonly BridgeValorAtualService _bridgeValorAtualService;
    private readonly ProcessoHealthScoreService _processoHealthScoreService;
    private readonly InterpretacaoProcessoService _interpretacaoProcessoService;

    public ProcessoInteligenciaService(
        AppDbContext context,
        BridgeValorAtualService bridgeValorAtualService,
        ProcessoHealthScoreService processoHealthScoreService,
        InterpretacaoProcessoService interpretacaoProcessoService)
    {
        _context = context;
        _bridgeValorAtualService = bridgeValorAtualService;
        _processoHealthScoreService = processoHealthScoreService;
        _interpretacaoProcessoService = interpretacaoProcessoService;
    }

    public async Task<AnaliseProcessoResultado> AnalisarProcesso(int processoId)
    {
        var processo = _context.ProcessosIa
            .Include(x => x.Tags)
            .FirstOrDefault(x => x.Id == processoId);

        if (processo == null)
            throw new Exception("Processo não encontrado.");

        if (!processo.Tags.Any())
            throw new Exception("Processo não possui tags vinculadas.");

        var tagNames = processo.Tags
            .Select(x => x.TagName)
            .Distinct()
            .ToList();

        var valoresAtuais =
            await _bridgeValorAtualService.BuscarValoresAtuais(tagNames);

        var resultados = new List<AnaliseTagProcessoResultado>();

        foreach (var tagProcesso in processo.Tags)
        {
            var perfil = _context.PerfisIa
                .FirstOrDefault(x =>
                    x.ClienteId == processo.ClienteId &&
                    x.TagName == tagProcesso.TagName);

            var contexto = _context.TagsContextoIa
                .FirstOrDefault(x =>
                    x.ClienteId == processo.ClienteId &&
                    x.TagName == tagProcesso.TagName);

            var valorAtual = valoresAtuais
                .FirstOrDefault(x =>
                    string.Equals(x.TagName, tagProcesso.TagName, StringComparison.OrdinalIgnoreCase));

            if (perfil == null)
            {
                resultados.Add(new AnaliseTagProcessoResultado
                {
                    TagName = tagProcesso.TagName,
                    PapelDaTag = tagProcesso.PapelDaTag,
                    Descricao = contexto?.Descricao,
                    Criticidade = contexto?.Criticidade,
                    PerfilTreinado = false,
                    Encontrado = valorAtual?.Encontrado ?? false,
                    Status = "sem_perfil",
                    Mensagem = "Tag ainda não possui perfil treinado."
                });

                continue;
            }

            if (valorAtual == null || !valorAtual.Encontrado)
            {
                resultados.Add(new AnaliseTagProcessoResultado
                {
                    TagName = tagProcesso.TagName,
                    PapelDaTag = tagProcesso.PapelDaTag,
                    Descricao = contexto?.Descricao,
                    Criticidade = contexto?.Criticidade,
                    PerfilTreinado = true,
                    Encontrado = false,
                    Media = perfil.Media,
                    DesvioPadrao = perfil.DesvioPadrao,
                    Status = "sem_valor_atual",
                    Mensagem = "Valor atual não encontrado no Bridge."
                });

                continue;
            }

            var limiteInferior = perfil.Media - (3 * perfil.DesvioPadrao);
            var limiteSuperior = perfil.Media + (3 * perfil.DesvioPadrao);

            var ehAnomalia =
                valorAtual.Valor < limiteInferior ||
                valorAtual.Valor > limiteSuperior;

            var score = perfil.DesvioPadrao > 0
                ? Math.Abs(valorAtual.Valor - perfil.Media) / perfil.DesvioPadrao
                : 0;

            resultados.Add(new AnaliseTagProcessoResultado
            {
                TagName = tagProcesso.TagName,
                PapelDaTag = tagProcesso.PapelDaTag,
                Descricao = contexto?.Descricao,
                Criticidade = contexto?.Criticidade,

                ValorAtual = valorAtual.Valor,
                Media = perfil.Media,
                DesvioPadrao = perfil.DesvioPadrao,
                LimiteInferior = limiteInferior,
                LimiteSuperior = limiteSuperior,
                Score = score,

                EhAnomalia = ehAnomalia,
                PerfilTreinado = true,
                Encontrado = true,
                Status = ehAnomalia ? "anomala" : "normal",
                Mensagem = ehAnomalia
                    ? "Tag fora do perfil esperado dentro do processo."
                    : "Tag dentro do perfil esperado dentro do processo."
            });
        }

        var totalTags = resultados.Count;
        var totalAnomalas = resultados.Count(x => x.EhAnomalia);
        var totalSemPerfil = resultados.Count(x => !x.PerfilTreinado);

        var saudeProcesso = _processoHealthScoreService.Calcular(resultados);

        var resultadoFinal = new AnaliseProcessoResultado
        {
            ProcessoId = processo.Id,
            ClienteId = processo.ClienteId,
            NomeProcesso = processo.Nome,
            Area = processo.Area,
            Criticidade = processo.Criticidade,
            Objetivo = processo.Objetivo,

            TotalTags = totalTags,
            TotalTagsAnomalas = totalAnomalas,
            TotalTagsSemPerfil = totalSemPerfil,

            ScoreSaude = saudeProcesso.ScoreSaude,
            ClassificacaoSaude = saudeProcesso.Classificacao,
            MensagemSaude = saudeProcesso.Mensagem,

            Resumo = MontarResumoProcesso(
                processo,
                totalTags,
                totalAnomalas,
                totalSemPerfil,
                saudeProcesso.ScoreSaude,
                saudeProcesso.Classificacao
            ),

            Tags = resultados
        };

        resultadoFinal.Interpretacao =
            _interpretacaoProcessoService.Interpretar(resultadoFinal);

        return resultadoFinal;
    }

    private string MontarResumoProcesso(
        ProcessoIa processo,
        int totalTags,
        int totalAnomalas,
        int totalSemPerfil,
        int scoreSaude,
        string classificacao)
    {
        return
            $"Processo {processo.Nome}: {totalAnomalas} de {totalTags} tag(s) apresentam anomalia. " +
            $"Saúde do processo: {scoreSaude}/100 ({classificacao}). " +
            (totalSemPerfil > 0
                ? $"{totalSemPerfil} tag(s) ainda não possuem perfil treinado."
                : "Todas as tags analisadas possuem perfil treinado.");
    }
}

public class AnaliseProcessoResultado
{
    public int ProcessoId { get; set; }
    public string ClienteId { get; set; } = "";
    public string NomeProcesso { get; set; } = "";

    public string? Area { get; set; }
    public string? Criticidade { get; set; }
    public string? Objetivo { get; set; }

    public int TotalTags { get; set; }
    public int TotalTagsAnomalas { get; set; }
    public int TotalTagsSemPerfil { get; set; }

    public int ScoreSaude { get; set; }
    public string ClassificacaoSaude { get; set; } = "";
    public string MensagemSaude { get; set; } = "";

    public string Resumo { get; set; } = "";
    public string Interpretacao { get; set; } = "";

    public List<AnaliseTagProcessoResultado> Tags { get; set; } = new();
}

public class AnaliseTagProcessoResultado
{
    public string TagName { get; set; } = "";
    public string? PapelDaTag { get; set; }
    public string? Descricao { get; set; }
    public string? Criticidade { get; set; }

    public double ValorAtual { get; set; }
    public double Media { get; set; }
    public double DesvioPadrao { get; set; }
    public double LimiteInferior { get; set; }
    public double LimiteSuperior { get; set; }
    public double Score { get; set; }

    public bool EhAnomalia { get; set; }
    public bool PerfilTreinado { get; set; }
    public bool Encontrado { get; set; }

    public string Status { get; set; } = "";
    public string Mensagem { get; set; } = "";
}