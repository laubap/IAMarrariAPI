using System.Text.Json.Serialization;

public class ProcessoTagIa
{
    public int Id { get; set; }

    public int ProcessoIaId { get; set; }
    public string ClienteId { get; set; } = "";

    public string TagName { get; set; } = "";
    public string? PapelDaTag { get; set; }

    [JsonIgnore]
    public ProcessoIa? ProcessoIa { get; set; }
}