public class AnomaliaResponse
{
    public string ClienteId { get; set; } = "";
    public string TagName { get; set; } = "";
    public string TipoTag { get; set; } = "";

    public bool EhAnomalia { get; set; }
    public float Score { get; set; }

    public string Mensagem { get; set; } = "";

    public ResumoAnomaliaDto Resumo { get; set; } = new();
    public PerfilComportamentalDto Perfil { get; set; } = new();
    public ContextoOperacionalDto? Contexto { get; set; }
    public List<DependenciaAnalisadaDto> Dependencias { get; set; } = new();
    public RiscoProcessoDto Risco { get; set; } = new();
    public TendenciasDto Tendencias { get; set; } = new();
    public ProcessoDto? Processo { get; set; }
}

public class ResumoAnomaliaDto
{
    public string Status { get; set; } = "";
    public string Conclusao { get; set; } = "";
}

public class PerfilComportamentalDto
{
    public double ValorRecebido { get; set; }
    public double MediaHistorica { get; set; }
    public double LimiteInferior { get; set; }
    public double LimiteSuperior { get; set; }
    public double Score { get; set; }
}

public class ContextoOperacionalDto
{
    public string? Descricao { get; set; }
    public string? Criticidade { get; set; }
    public string? Equipamento { get; set; }
    public string? Area { get; set; }

    public List<string> Impactos { get; set; } = new();
    public List<string> CausasProvaveis { get; set; } = new();
    public List<string> AcoesRecomendadas { get; set; } = new();
    public List<string> ModosOperacao { get; set; } = new();

    public string? ObservacaoModoOperacao { get; set; }
}

public class DependenciaAnalisadaDto
{
    public string TagName { get; set; } = "";
    public string? Descricao { get; set; }

    public double ValorAtual { get; set; }
    public double Media { get; set; }
    public double LimiteInferior { get; set; }
    public double LimiteSuperior { get; set; }

    public bool EhAnomalia { get; set; }
    public string Status { get; set; } = "";

    public string? TipoRelacao { get; set; }
    public string? Impacto { get; set; }
    public string? Criticidade { get; set; }
    public string? Equipamento { get; set; }
    public string? Area { get; set; }

    public string Interpretacao { get; set; } = "";
}

public class RiscoProcessoDto
{
    public int Score { get; set; }
    public string Classificacao { get; set; } = "";
    public string Mensagem { get; set; } = "";
    public List<string> Fatores { get; set; } = new();
}

public class TendenciasDto
{
    public string TendenciaRisco { get; set; } = "";
    public string MensagemTendenciaRisco { get; set; } = "";

    public string TendenciaValor { get; set; } = "";
    public string MensagemTendenciaValor { get; set; } = "";
}

public class ProcessoDto
{
    public string Nome { get; set; } = "";
    public string? Descricao { get; set; }
    public string? Area { get; set; }
    public string? Criticidade { get; set; }

    public string? Objetivo { get; set; }

    public string? CondicaoNormal { get; set; }

    public string? ConsequenciasFalha { get; set; }

    public string? ProcedimentoRecomendado { get; set; }
}