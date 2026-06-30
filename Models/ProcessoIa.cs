// Modelo que representa um processo no sistema de IA.
// Contém informações de negócio, criticidade, ações recomendadas e os relacionamentos
// com tags e equipamentos.
public class ProcessoIa
{
    public int Id { get; set; }

    // Identificador do cliente proprietário do processo.
    public string ClienteId { get; set; } = "";

    // Nome do processo.
    public string Nome { get; set; } = "";

    // Descrição e metadados do processo.
    public string? Descricao { get; set; }
    public string? Area { get; set; }
    public string? Criticidade { get; set; }
    public string? Objetivo { get; set; }
    public string? CondicaoNormal { get; set; }
    public string? ConsequenciasFalha { get; set; }
    public string? ProcedimentoRecomendado { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.Now;
    public DateTime DataAtualizacao { get; set; } = DateTime.Now;

    // Tags associadas a esse processo.
    public List<ProcessoTagIa> Tags { get; set; } = new();

    // Equipamentos vinculados a esse processo.
    public List<ProcessoEquipamentoIa> Equipamentos { get; set; } = new();
}