public class TagDependenciaIa
{
    public int Id { get; set; }

    public string ClienteId { get; set; } = "";

    // Tag principal
    public string TagName { get; set; } = "";

    // Tag que depende/relaciona com a principal
    public string TagDependente { get; set; } = "";

    public string TipoRelacao { get; set; } = "";
    public string Impacto { get; set; } = "";
    public string? Descricao { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.Now;
}