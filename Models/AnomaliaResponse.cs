public class AnomaliaResponse
{
    public string ClienteId { get; set; } = "";
    public string TagName { get; set; } = "";
    public bool EhAnomalia { get; set; }
    public float Score { get; set; }
    public string Mensagem { get; set; } = "";
}