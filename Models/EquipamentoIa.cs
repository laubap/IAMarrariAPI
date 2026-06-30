// Modelo que representa um equipamento no sistema de IA.
// Contém informações básicas do equipamento, metadados de manutenção
// e relacionamentos com tags e processos.
public class EquipamentoIa
{
    public int Id { get; set; }

    // Identificador do cliente a que o equipamento pertence.
    public string ClienteId { get; set; } = "";

    // Nome do equipamento.
    public string Nome { get; set; } = "";

    // Informações adicionais sobre o equipamento.
    public string? Descricao { get; set; }
    public string? Area { get; set; }
    public string? TipoEquipamento { get; set; }
    public string? Criticidade { get; set; }

    public string? Fabricante { get; set; }
    public string? Modelo { get; set; }

    // Dados de manutenção e observações.
    public DateTime? DataUltimaManutencao { get; set; }
    public string? Observacoes { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.Now;
    public DateTime DataAtualizacao { get; set; } = DateTime.Now;

    // Tags associadas a este equipamento.
    public List<EquipamentoTagIa> Tags { get; set; } = new();

    // Processos aos quais este equipamento está vinculado.
    public List<ProcessoEquipamentoIa> Processos { get; set; } = new();
    
}