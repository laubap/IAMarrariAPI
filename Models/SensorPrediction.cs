using Microsoft.ML.Data;

public class SensorPrediction
{
    [ColumnName("PredictedLabel")]
    public bool EhAnomalia { get; set; }

    public float Score { get; set; }
}