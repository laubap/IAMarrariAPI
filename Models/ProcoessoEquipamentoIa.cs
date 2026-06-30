using System.Text.Json.Serialization;

// Modelo que representa o vínculo entre um processo e um equipamento.
// Armazena informações sobre o papel do equipamento no processo e observações.
public class ProcessoEquipamentoIa
{
    public int Id { get; set; }

    // FK para o processo associado.
    public int ProcessoIaId { get; set; }

    // FK para o equipamento associado.
    public int EquipamentoIaId { get; set; }

    // Descreve o papel do equipamento dentro do processo.
    public string? PapelNoProcesso { get; set; }

    // Observações adicionais sobre o vínculo.
    public string? Observacao { get; set; }

    [JsonIgnore]
    public ProcessoIa? Processo { get; set; }

    [JsonIgnore]
    public EquipamentoIa? Equipamento { get; set; }
}