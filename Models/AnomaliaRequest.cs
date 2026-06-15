public class AnomaliaRequest
{
    public string ClienteId { get; set; } = "";
    public string TagName { get; set; } = "";

    public string TipoTag { get; set; } = "";
    public DateTime DataHora { get; set; }
    public float Valor { get; set; }
}