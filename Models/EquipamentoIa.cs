public class EquipamentoIa
{
    public int Id { get; set; }

    public string ClienteId { get; set; } = "";
    public string Nome { get; set; } = "";

    public string? Descricao { get; set; }
    public string? Area { get; set; }
    public string? TipoEquipamento { get; set; }
    public string? Criticidade { get; set; }

    public string? Fabricante { get; set; }
    public string? Modelo { get; set; }

    public DateTime? DataUltimaManutencao { get; set; }
    public string? Observacoes { get; set; }

    public int? ProcessoIaId { get; set; }
    public ProcessoIa? ProcessoIa { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.Now;
    public DateTime DataAtualizacao { get; set; } = DateTime.Now;

    public List<EquipamentoTagIa> Tags { get; set; } = new();
}