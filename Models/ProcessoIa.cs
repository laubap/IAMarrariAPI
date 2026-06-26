public class ProcessoIa
{
    public int Id { get; set; }

    public string ClienteId { get; set; } = "";
    public string Nome { get; set; } = "";

    public string? Descricao { get; set; }
    public string? Area { get; set; }
    public string? Criticidade { get; set; }
    public string? Objetivo { get; set; }
    public string? CondicaoNormal { get; set; }
    public string? ConsequenciasFalha { get; set; }
    public string? ProcedimentoRecomendado { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.Now;
    public DateTime DataAtualizacao { get; set; } = DateTime.Now;

    public List<ProcessoTagIa> Tags { get; set; } = new();
}