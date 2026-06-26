public class TagContextoIa
{
    public int Id { get; set; }


    public string ClienteId { get; set; } = "";
    public string TagName { get; set; } = "";


    public string? TipoRepresentacao { get; set; }
    public string? Descricao { get; set; }
    public string? Criticidade { get; set; }


//Salvos como JSON 
    public string? Impactos { get; set; }
    public string? TagsRelacionadas { get; set; }
    public string? ModosOperacao { get; set; }
    public string? CausasProvaveis { get; set; }
     public string? AcoesRecomendadas { get; set; }

     
    public string? Equipamento { get; set; }
    public string? Area { get; set; }
    public string? ObservacaoModoOperacao { get; set; }


    public bool ContextoCompleto {get; set;}


    public DateTime DataCriacao { get; set; } = DateTime.Now;
    public DateTime DataAtualizacao { get; set; } = DateTime.Now;
}