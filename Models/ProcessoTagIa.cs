using System.Text.Json.Serialization;

// Modelo que representa a associação de uma tag a um processo.
// Cada registro indica que uma tag faz parte de um processo e define o papel dessa tag.
public class ProcessoTagIa
{
    public int Id { get; set; }

    // FK para o processo associado.
    public int ProcessoIaId { get; set; }

    // Cliente proprietário do processo e da tag.
    public string ClienteId { get; set; } = "";

    // Nome da tag vinculada ao processo.
    public string TagName { get; set; } = "";

    // Papel que essa tag desempenha dentro do processo.
    public string? PapelDaTag { get; set; }

    [JsonIgnore]
    public ProcessoIa? ProcessoIa { get; set; }
}