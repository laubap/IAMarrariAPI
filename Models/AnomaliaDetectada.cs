public class AnomaliaDetectada
{
    public int Id { get; set; }

    public string ClienteId { get; set; } = "";

    public string TagName { get; set; } = "";

    public string TipoTag { get; set; } = "";

    public double ValorRecebido { get; set; }

    public double Score { get; set; }

    public bool EhAnomalia { get; set; }

    public string Mensagem { get; set; } = "";

    public string? Criticidade { get; set; }

    public List<AnomaliaDependenciaDetectada> Dependencias { get; set; } = new();

    public int ScoreRiscoProcesso { get; set; }
    
    public string? ClassificacaoRisco { get; set; }

    public string? TendenciaRisco { get; set; }

    public string? TendenciaValor { get; set; }

    public DateTime DataDeteccao { get; set; } = DateTime.Now;
}