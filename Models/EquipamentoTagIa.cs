using System.Text.Json.Serialization;

public class EquipamentoTagIa
{
    public int Id { get; set; }

    public int EquipamentoIaId { get; set; }
    public string ClienteId { get; set; } = "";

    public string TagName { get; set; } = "";
    public string? PapelDaTag { get; set; }

    [JsonIgnore]
    public EquipamentoIa? EquipamentoIa { get; set; }
}