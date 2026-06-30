// Modelo que representa o perfil comportamental de uma tag.
// Ele armazena estatísticas calculadas durante o treinamento do perfil para
// identificar comportamentos esperados e anomalias.
public class TagPerfilIa
{
    public int Id { get; set; }

    // Identificador do cliente proprietário da tag.
    public string ClienteId { get; set; } = "";

    // Nome da tag cujo perfil foi treinado.
    public string TagName { get; set; } = "";

    // Média histórica dos valores da tag.
    public double Media { get; set; }

    // Desvio padrão usado para definir limites de anomalia.
    public double DesvioPadrao { get; set; }

    public double Minimo { get; set; }

    public double Maximo { get; set; }

    // Amplitude entre mínimo e máximo.
    public double Amplitude { get; set; }
    
    // Percentual de leituras zero no histórico.
    public double PercentualZeros { get; set; }

    // Variação média entre leituras.
    public double VariacaoMedia { get; set; }

    // Quantidade de picos detectados no histórico.
    public int QuantidadePicos { get; set; }

    // Total de registros históricos usados no treinamento.
    public int TotalRegistrosHistorico { get; set; }

    // Total de registros efetivamente utilizados após filtros.
    public int TotalRegistrosUsados { get; set; }
    
    // Quantidade de outliers removidos do histórico.
    public int TotalOutliersRemovidos { get; set; }

    // Data em que o perfil foi treinado.
    public DateTime DataTreinamento { get; set; }
}