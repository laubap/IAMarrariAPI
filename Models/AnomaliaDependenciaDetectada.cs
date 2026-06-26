using System.Text.Json.Serialization;

public class AnomaliaDependenciaDetectada
{
    public int Id { get; set; }

    public int AnomaliaDetectadaId { get; set; }

    public string TagDependente { get; set; } = "";

    public double ValorAtual { get; set; }
    public double Media { get; set; }
    public double LimiteInferior { get; set; }
    public double LimiteSuperior { get; set; }

    public bool EhAnomalia { get; set; }
    public string Status { get; set; } = "";

    public string? TipoRelacao { get; set; }
    public string? Impacto { get; set; }
    public string? DescricaoRelacao { get; set; }

    public DateTime DataDeteccao { get; set; } = DateTime.Now;

    [JsonIgnore]
    public AnomaliaDetectada? AnomaliaDetectada { get; set; }
}