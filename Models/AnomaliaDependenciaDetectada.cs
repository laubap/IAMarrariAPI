using System.Text.Json.Serialization;

// Representa uma dependência de tag detectada dentro de uma anomalia.
// Cada registro guarda a tag dependente, seus valores esperados, status e metadados de relação.
public class AnomaliaDependenciaDetectada
{
    public int Id { get; set; }

    // FK para a anomalia principal à qual esta dependência pertence.
    public int AnomaliaDetectadaId { get; set; }

    public string TagDependente { get; set; } = "";

    // Valores usados para avaliar o comportamento da tag dependente.
    public double ValorAtual { get; set; }
    public double Media { get; set; }
    public double LimiteInferior { get; set; }
    public double LimiteSuperior { get; set; }

    public bool EhAnomalia { get; set; }
    public string Status { get; set; } = "";

    // Informações de relação entre tags, como tipo, impacto e descrição.
    public string? TipoRelacao { get; set; }
    public string? Impacto { get; set; }
    public string? DescricaoRelacao { get; set; }

    // Data em que a dependência foi detectada.
    public DateTime DataDeteccao { get; set; } = DateTime.Now;

    [JsonIgnore]
    public AnomaliaDetectada? AnomaliaDetectada { get; set; }
}