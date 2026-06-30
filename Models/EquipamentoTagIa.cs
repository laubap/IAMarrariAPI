using System.Text.Json.Serialization;

// Modelo que representa a associação de uma tag a um equipamento.
// Guarda metadados da tag dentro do contexto do equipamento e mantém a referência ao equipamento pai.
public class EquipamentoTagIa
{
    public int Id { get; set; }

    // FK para o equipamento associado.
    public int EquipamentoIaId { get; set; }

    // Cliente proprietário da tag e do equipamento.
    public string ClienteId { get; set; } = "";

    // Nome da tag vinculada ao equipamento.
    public string TagName { get; set; } = "";
    public string? PapelDaTag { get; set; }

    // Escopo ou contexto em que a tag é usada no equipamento.
    public string? Escopo { get; set; }

    [JsonIgnore]
    public EquipamentoIa? EquipamentoIa { get; set; }
}