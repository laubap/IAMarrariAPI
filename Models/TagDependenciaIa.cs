// Modelo que representa uma relação de dependência entre tags.
// Ele registra qual tag depende da tag principal, o tipo de relação, impacto e descrição.
public class TagDependenciaIa
{
    public int Id { get; set; }

    // Cliente proprietário das tags.
    public string ClienteId { get; set; } = "";

    // Tag principal da relação.
    public string TagName { get; set; } = "";

    // Tag dependente ou relacionada à tag principal.
    public string TagDependente { get; set; } = "";

    // Tipo de relação entre as tags.
    public string TipoRelacao { get; set; } = "";

    // Impacto da dependência no contexto da tag principal.
    public string Impacto { get; set; } = "";

    // Descrição opcional da dependência.
    public string? Descricao { get; set; }

    // Data em que a dependência foi cadastrada.
    public DateTime DataCriacao { get; set; } = DateTime.Now;
}