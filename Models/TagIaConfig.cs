using System.Data;

public class TagIaConfig
{
    public int Id {get; set;}
    public string ClienteId {get; set;} = "";

    public string TagName {get; set;} = "";

    public string TipoTag {get; set;} = "";

    public bool IaAtiva {get; set;} = true;

    public DateTime DataConfiguracao {get; set;} = DateTime.Now;
}