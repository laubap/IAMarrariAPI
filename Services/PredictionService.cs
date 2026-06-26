using System.Text.Json;

public class PredictionService
{
    private readonly AppDbContext _context;
    private readonly AnaliseRelacionadasService _analiseRelacionadasService;
    private readonly AnomaliaHistoricoService _historicoService;
    private readonly RiskScoreService _riskScoreService;
    private readonly ExplicacaoRiscoService _explicacaoRiscoService;
    private readonly TendenciaRiscoService _tendenciaRiscoService;
    private readonly TendenciaValorService _tendenciaValorService;
    private readonly InterpretacaoDependenciasService _interpretacaoDependenciasService;
    private readonly ProcessoAnaliseService _processoAnaliseService;

    public PredictionService(
        AppDbContext context,
        AnaliseRelacionadasService analiseRelacionadasService,
        AnomaliaHistoricoService historicoService,
        RiskScoreService riskScoreService,
        ExplicacaoRiscoService explicacaoRiscoService,
        TendenciaRiscoService tendenciaRiscoService,
        TendenciaValorService tendenciaValorService,
        InterpretacaoDependenciasService interpretacaoDependenciasService,
        ProcessoAnaliseService processoAnaliseService)
    {
        _context = context;
        _analiseRelacionadasService = analiseRelacionadasService;
        _historicoService = historicoService;
        _riskScoreService = riskScoreService;
        _explicacaoRiscoService = explicacaoRiscoService;
        _tendenciaRiscoService = tendenciaRiscoService;
        _tendenciaValorService = tendenciaValorService;
        _interpretacaoDependenciasService = interpretacaoDependenciasService;
        _processoAnaliseService = processoAnaliseService;
    }

    public async Task<AnomaliaResponse> Detectar(AnomaliaRequest request)
    {
        var config = _context.TagsIaConfig
            .FirstOrDefault(x =>
                x.ClienteId == request.ClienteId &&
                x.TagName == request.TagName &&
                x.IaAtiva);

        if (config == null)
            throw new Exception($"IA não configurada para a tag {request.TagName} do cliente {request.ClienteId}.");

        var perfil = _context.PerfisIa
            .FirstOrDefault(x =>
                x.ClienteId == request.ClienteId &&
                x.TagName == request.TagName);

        if (perfil == null)
            throw new Exception($"Perfil da tag {request.TagName} ainda não foi treinado. Execute o treinamento de perfil antes da detecção.");

        var contexto = _context.TagsContextoIa
            .FirstOrDefault(x =>
                x.ClienteId == request.ClienteId &&
                x.TagName == request.TagName);

        var processo = _processoAnaliseService.BuscarProcessoDaTag(
            request.ClienteId,
            request.TagName
        );

        var limiteInferior = perfil.Media - (3 * perfil.DesvioPadrao);
        var limiteSuperior = perfil.Media + (3 * perfil.DesvioPadrao);

        var ehAnomalia =
            request.Valor < limiteInferior ||
            request.Valor > limiteSuperior;

        var distanciaDaMedia = Math.Abs(request.Valor - perfil.Media);

        var score = perfil.DesvioPadrao > 0
            ? distanciaDaMedia / perfil.DesvioPadrao
            : 0;

        var analiseRelacionadas = await _analiseRelacionadasService.Analisar(
            request.ClienteId,
            request.TagName
        );

        var riscoProcesso = _riskScoreService.Calcular(
            ehAnomalia,
            score,
            contexto?.Criticidade,
            analiseRelacionadas
        );

        var fatoresRisco = _explicacaoRiscoService.GerarFatores(
            score,
            contexto,
            analiseRelacionadas,
            riscoProcesso
        );

        var tendenciaRisco = _tendenciaRiscoService.Analisar(
            request.ClienteId,
            request.TagName
        );

        var tendenciaValor = _tendenciaValorService.Analisar(
            request.TagName,
            contexto?.TipoRepresentacao ?? "perfil-comportamental"
        );

        var mensagem = MontarMensagem(
            request,
            perfil,
            contexto,
            ehAnomalia,
            limiteInferior,
            limiteSuperior,
            score,
            analiseRelacionadas,
            riscoProcesso,
            fatoresRisco,
            tendenciaRisco,
            tendenciaValor
        );

        _historicoService.Registrar(
            request.ClienteId,
            request.TagName,
            contexto?.TipoRepresentacao ?? "perfil-comportamental",
            request.Valor,
            score,
            ehAnomalia,
            mensagem,
            contexto?.Criticidade,
            analiseRelacionadas,
            riscoProcesso.ScoreRisco,
            riscoProcesso.Classificacao,
            tendenciaRisco.Tendencia,
            tendenciaValor.Tendencia
        );

        return new AnomaliaResponse
        {
            ClienteId = request.ClienteId,
            TagName = request.TagName,
            TipoTag = contexto?.TipoRepresentacao ?? "perfil-comportamental",
            EhAnomalia = ehAnomalia,
            Score = (float)score,

            Mensagem = mensagem,

            Resumo = new ResumoAnomaliaDto
            {
                Status = ehAnomalia ? "Anomalia detectada" : "Comportamento normal",
                Conclusao = riscoProcesso.Mensagem
            },

            Perfil = new PerfilComportamentalDto
            {
                ValorRecebido = request.Valor,
                MediaHistorica = perfil.Media,
                LimiteInferior = limiteInferior,
                LimiteSuperior = limiteSuperior,
                Score = score
            },

            Contexto = contexto == null ? null : new ContextoOperacionalDto
            {
                Descricao = contexto.Descricao,
                Criticidade = contexto.Criticidade,
                Equipamento = contexto.Equipamento,
                Area = contexto.Area,
                ObservacaoModoOperacao = contexto.ObservacaoModoOperacao,

                Impactos = LerLista(contexto.Impactos),
                CausasProvaveis = LerLista(contexto.CausasProvaveis),
                AcoesRecomendadas = LerLista(contexto.AcoesRecomendadas),
                ModosOperacao = LerLista(contexto.ModosOperacao)
            },

            Processo = processo == null
            ? null
            : new ProcessoDto
            {
                Nome = processo.Nome,
                Descricao = processo.Descricao,
                Area = processo.Area,
                Criticidade = processo.Criticidade,
                Objetivo = processo.Objetivo,
                CondicaoNormal = processo.CondicaoNormal,
                ConsequenciasFalha = processo.ConsequenciasFalha,
                ProcedimentoRecomendado = processo.ProcedimentoRecomendado
            },

            Dependencias = analiseRelacionadas.Resultados.Select(item => new DependenciaAnalisadaDto
            {
                TagName = item.TagName,
                Descricao = item.Descricao,

                ValorAtual = item.ValorAtual,
                Media = item.Media,
                LimiteInferior = item.LimiteInferior,
                LimiteSuperior = item.LimiteSuperior,

                EhAnomalia = item.EhAnomalia,
                Status = item.EhAnomalia ? "anômala" : "normal",

                TipoRelacao = item.TipoRelacao,
                Impacto = item.Impacto,
                Criticidade = item.Criticidade,
                Equipamento = item.Equipamento,
                Area = item.Area,

                Interpretacao = _interpretacaoDependenciasService.Interpretar(item, contexto)}).ToList(),
                Risco = new RiscoProcessoDto
                {
                    Score = riscoProcesso.ScoreRisco,
                    Classificacao = riscoProcesso.Classificacao,
                    Mensagem = riscoProcesso.Mensagem,
                    Fatores = fatoresRisco
                },

                Tendencias = new TendenciasDto
                {
                        TendenciaRisco = tendenciaRisco.Tendencia,
                        MensagemTendenciaRisco = tendenciaRisco.Mensagem,

                        TendenciaValor = tendenciaValor.Tendencia,
                        MensagemTendenciaValor = tendenciaValor.Mensagem
                },
        };
    }

    private string MontarMensagem(
        AnomaliaRequest request,
        TagPerfilIa perfil,
        TagContextoIa? contexto,
        bool ehAnomalia,
        double limiteInferior,
        double limiteSuperior,
        double score,
        AnaliseRelacionadasResultado analiseRelacionadas,
        ResultadoRiscoProcesso riscoProcesso,
        List<string> fatoresRisco,
        ResultadoTendenciaRisco tendenciaRisco,
        ResultadoTendenciaValor tendenciaValor)
    {
        var nomeTag =
            !string.IsNullOrWhiteSpace(contexto?.Descricao)
                ? $"{request.TagName} ({contexto.Descricao})"
                : request.TagName;

        var mensagem = ehAnomalia
            ? $"Anomalia detectada na tag {nomeTag}. O valor {request.Valor} está fora do perfil comportamental esperado."
            : $"Comportamento normal na tag {nomeTag}. O valor {request.Valor} está dentro do perfil esperado.";

        mensagem += $" Média histórica: {perfil.Media:F2}.";
        mensagem += $" Limite esperado: {limiteInferior:F2} até {limiteSuperior:F2}.";
        mensagem += $" Score: {score:F2}.";

        if (contexto == null)
        {
            mensagem += " Nenhum contexto operacional foi cadastrado para esta tag.";
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(contexto.Criticidade))
                mensagem += $" Criticidade: {contexto.Criticidade}.";

            var impactos = LerLista(contexto.Impactos);
            if (impactos.Any())
                mensagem += $" Possíveis impactos: {string.Join(", ", impactos)}.";

            var causas = LerLista(contexto.CausasProvaveis);
            if (ehAnomalia && causas.Any())
                mensagem += $" Causas prováveis informadas: {string.Join(", ", causas)}.";

            var acoes = LerLista(contexto.AcoesRecomendadas);
            if (ehAnomalia && acoes.Any())
                mensagem += $" Ações recomendadas: {string.Join(", ", acoes)}.";

            var modos = LerLista(contexto.ModosOperacao);
            if (modos.Any())
                mensagem += $" Modos de operação relevantes: {string.Join(", ", modos)}.";

            if (!string.IsNullOrWhiteSpace(contexto.Equipamento))
                mensagem += $" Equipamento: {contexto.Equipamento}.";

            if (!string.IsNullOrWhiteSpace(contexto.Area))
                mensagem += $" Área: {contexto.Area}.";

            if (!string.IsNullOrWhiteSpace(contexto.ObservacaoModoOperacao))
                mensagem += $" Observação operacional: {contexto.ObservacaoModoOperacao}.";
        }

        mensagem += $" Análise de tags relacionadas: {analiseRelacionadas.Resumo}";

        foreach (var item in analiseRelacionadas.Resultados)
        {
            var interpretacao =
            _interpretacaoDependenciasService.Interpretar(item, contexto);

            mensagem += $" [{item.TagName}";

            if (item.PossuiContexto && !string.IsNullOrWhiteSpace(item.Descricao))
                mensagem += $" ({item.Descricao})";

            mensagem += $": valor atual {item.ValorAtual:F2}, média {item.Media:F2}, limite {item.LimiteInferior:F2} até {item.LimiteSuperior:F2}, status: {(item.EhAnomalia ? "anômala" : "normal")}.";

            if (!string.IsNullOrWhiteSpace(item.TipoRelacao))
                mensagem += $" Relação: {item.TipoRelacao}.";

            if (!string.IsNullOrWhiteSpace(item.Impacto))
                mensagem += $" Impacto: {item.Impacto}.";

            if (item.PossuiContexto)
            {
                if (!string.IsNullOrWhiteSpace(item.Criticidade))
                    mensagem += $" Criticidade: {item.Criticidade}.";

                if (!string.IsNullOrWhiteSpace(item.Equipamento))
                    mensagem += $" Equipamento: {item.Equipamento}.";

                if (!string.IsNullOrWhiteSpace(item.Area))
                    mensagem += $" Área: {item.Area}.";
            }

            mensagem += $" Interpretação: {interpretacao}]";
        }

        mensagem += $" Risco do processo: {riscoProcesso.ScoreRisco}/100.";
        mensagem += $" Classificação do risco: {riscoProcesso.Classificacao}.";
        mensagem += $" {riscoProcesso.Mensagem}";

        if (fatoresRisco.Any())
        {
            mensagem += " Fatores que contribuíram para o risco: ";

            foreach (var fator in fatoresRisco)
            {
                mensagem += $" [{fator}]";
            }
        }

        mensagem += $" Tendência do risco: {tendenciaRisco.Tendencia}.";
        mensagem += $" {tendenciaRisco.Mensagem}";
        mensagem += $" Tendência do valor da tag: {tendenciaValor.Tendencia}.";
        mensagem += $" {tendenciaValor.Mensagem}";

        return mensagem;
    }

    private List<string> LerLista(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new List<string>();

        try
        {
            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }
}