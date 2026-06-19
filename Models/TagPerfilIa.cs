public class TagPerfilIa
{
    public int Id { get; set; }

    public string ClienteId { get; set; } = "";

    public string TagName { get; set; } = "";

    public double Media { get; set; }

    public double DesvioPadrao { get; set; }

    public double Minimo { get; set; }

    public double Maximo { get; set; }

    public double Amplitude { get; set; }
    
    public double PercentualZeros { get; set; }

    public double VariacaoMedia { get; set; }

    public int QuantidadePicos { get; set; }

    public int TotalRegistrosHistorico { get; set; }

    public int TotalRegistrosUsados { get; set; }
    
    public int TotalOutliersRemovidos { get; set; }

    public DateTime DataTreinamento { get; set; }
}