using Microsoft.EntityFrameworkCore;

// Serviço de inteligência para análise de equipamentos.
// Ele busca o equipamento e suas tags, consulta valores atuais no Bridge,
// compara com os perfis treinados e produz um resultado de análise.
public class EquipamentoInteligenciaService
{
    private readonly AppDbContext _context;
    private readonly BridgeValorAtualService _bridgeValorAtualService;
    private readonly ProcessoHealthScoreService _healthScoreService;

    public EquipamentoInteligenciaService(
        AppDbContext context,
        BridgeValorAtualService bridgeValorAtualService,
        ProcessoHealthScoreService healthScoreService)
    {
        _context = context;
        _bridgeValorAtualService = bridgeValorAtualService;
        _healthScoreService = healthScoreService;
    }

    // Executa a análise de um equipamento específico pelo seu ID.
    // Retorna dados do equipamento, análise de cada tag vinculada e um score de saúde.
    public async Task<AnaliseEquipamentoResultado> AnalisarEquipamento(int equipamentoId)
    {
        var equipamento = _context.EquipamentosIa
            .Include(x => x.Tags)
            .Include(x => x.Processos)
                .ThenInclude(x => x.Processo)
            .FirstOrDefault(x => x.Id == equipamentoId);

        if (equipamento == null)
            throw new Exception("Equipamento não encontrado.");

        var tagNames = equipamento.Tags
            .Select(x => x.TagName)
            .Distinct()
            .ToList();

        if (!tagNames.Any())
            throw new Exception("Equipamento não possui tags vinculadas.");

        var valoresAtuais =
            await _bridgeValorAtualService.BuscarValoresAtuais(tagNames);

        var tagsAnalisadas = new List<AnaliseTagProcessoResultado>();

        foreach (var tagEquipamento in equipamento.Tags)
        {
            var perfil = _context.PerfisIa
                .FirstOrDefault(x =>
                    x.ClienteId == equipamento.ClienteId &&
                    x.TagName == tagEquipamento.TagName);

            var contexto = _context.TagsContextoIa
                .FirstOrDefault(x =>
                    x.ClienteId == equipamento.ClienteId &&
                    x.TagName == tagEquipamento.TagName);

            var valorAtual = valoresAtuais
                .FirstOrDefault(x =>
                    string.Equals(x.TagName, tagEquipamento.TagName, StringComparison.OrdinalIgnoreCase));

            if (perfil == null)
            {
                tagsAnalisadas.Add(new AnaliseTagProcessoResultado
                {
                    TagName = tagEquipamento.TagName,
                    PapelDaTag = tagEquipamento.PapelDaTag,
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
                tagsAnalisadas.Add(new AnaliseTagProcessoResultado
                {
                    TagName = tagEquipamento.TagName,
                    PapelDaTag = tagEquipamento.PapelDaTag,
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

            tagsAnalisadas.Add(new AnaliseTagProcessoResultado
            {
                TagName = tagEquipamento.TagName,
                PapelDaTag = tagEquipamento.PapelDaTag,
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
                    ? "Tag fora do perfil esperado para o equipamento."
                    : "Tag dentro do perfil esperado para o equipamento."
            });
        }

        var saude = _healthScoreService.Calcular(tagsAnalisadas);

        var resultado = new AnaliseEquipamentoResultado
        {
            EquipamentoId = equipamento.Id,
            ClienteId = equipamento.ClienteId,
            Nome = equipamento.Nome,
            Descricao = equipamento.Descricao,
            Area = equipamento.Area,
            TipoEquipamento = equipamento.TipoEquipamento,
            Criticidade = equipamento.Criticidade,
            Fabricante = equipamento.Fabricante,
            Modelo = equipamento.Modelo,
            DataUltimaManutencao = equipamento.DataUltimaManutencao,

            TotalTags = tagsAnalisadas.Count,
            TotalTagsAnomalas = tagsAnalisadas.Count(x => x.EhAnomalia),
            TotalTagsSemPerfil = tagsAnalisadas.Count(x => !x.PerfilTreinado),
            TotalTagsSemValorAtual = tagsAnalisadas.Count(x => x.Status == "sem_valor_atual"),

            ScoreSaude = saude.ScoreSaude,
            ClassificacaoSaude = saude.Classificacao,
            MensagemSaude = saude.Mensagem,

            Processos = equipamento.Processos.Select(x => new ProcessoResumoEquipamentoDto
            {
                ProcessoId = x.ProcessoIaId,
                Nome = x.Processo?.Nome,
                PapelNoProcesso = x.PapelNoProcesso,
                Observacao = x.Observacao
            }).ToList(),

            Tags = tagsAnalisadas
        };

        resultado.Interpretacao = GerarInterpretacao(resultado);

        return resultado;
    }

    // Gera uma interpretação textual do resultado da análise do equipamento.
    // A mensagem varia conforme anomalias detectadas e se existem tags sem perfil treinado.
    private string GerarInterpretacao(AnaliseEquipamentoResultado equipamento)
    {
        if (equipamento.TotalTagsAnomalas == 0 && equipamento.TotalTagsSemPerfil == 0)
        {
            return $"O equipamento {equipamento.Nome} apresenta operação estável. Todas as tags monitoradas estão dentro do perfil esperado.";
        }

        if (equipamento.TotalTagsAnomalas == 0 && equipamento.TotalTagsSemPerfil > 0)
        {
            return $"O equipamento {equipamento.Nome} não apresenta anomalias no momento, porém {equipamento.TotalTagsSemPerfil} tag(s) ainda não possuem perfil treinado, reduzindo a confiabilidade da análise.";
        }

        if (equipamento.TotalTagsAnomalas == 1)
        {
            return $"O equipamento {equipamento.Nome} apresenta uma anomalia isolada. Recomenda-se verificar a tag afetada antes de concluir falha no equipamento inteiro.";
        }

        return $"O equipamento {equipamento.Nome} apresenta múltiplas tags anômalas, indicando possível degradação do equipamento e necessidade de investigação prioritária.";
    }
}

// Modelo que representa o resultado completo da análise de um equipamento.
// Inclui metadados do equipamento, contagem de casos, score de saúde,
// interpretações e listas de processos e tags analisadas.
public class AnaliseEquipamentoResultado
{
    public int EquipamentoId { get; set; }
    public string ClienteId { get; set; } = "";

    public string Nome { get; set; } = "";
    public string? Descricao { get; set; }
    public string? Area { get; set; }
    public string? TipoEquipamento { get; set; }
    public string? Criticidade { get; set; }

    public string? Fabricante { get; set; }
    public string? Modelo { get; set; }
    public DateTime? DataUltimaManutencao { get; set; }

    public int TotalTags { get; set; }
    public int TotalTagsAnomalas { get; set; }
    public int TotalTagsSemPerfil { get; set; }
    public int TotalTagsSemValorAtual { get; set; }

    public int ScoreSaude { get; set; }
    public string ClassificacaoSaude { get; set; } = "";
    public string MensagemSaude { get; set; } = "";

    public string Interpretacao { get; set; } = "";

    public List<ProcessoResumoEquipamentoDto> Processos { get; set; } = new();
    public List<AnaliseTagProcessoResultado> Tags { get; set; } = new();
}

// Resumo dos processos relacionados ao equipamento, usado no resultado da análise.
public class ProcessoResumoEquipamentoDto
{
    public int ProcessoId { get; set; }
    public string? Nome { get; set; }
    public string? PapelNoProcesso { get; set; }
    public string? Observacao { get; set; }
}