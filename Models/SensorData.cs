using Microsoft.ML.Data;

public class SensorData
{
    [LoadColumn(1)]
    public float ValorAtual { get; set; }

    [LoadColumn(2)]
    public float MediaMovel5 { get; set; }

    [LoadColumn(3)]
    public float Variacao { get; set; }

    [LoadColumn(4)]
    public float MinJanela5 { get; set; }

    [LoadColumn(5)]
    public float MaxJanela5 { get; set; }

    [LoadColumn(6)]
    public float DesvioPadrao5 { get; set; }
}