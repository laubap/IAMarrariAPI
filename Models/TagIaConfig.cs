using System.Data;

// Modelo que representa a configuração de IA para uma tag.
// Ele define se a inteligência está ativa para a tag de um cliente e registra
// a data da configuração.
public class TagIaConfig
{
    public int Id {get; set;}

    // Identificador do cliente proprietário da tag.
    public string ClienteId {get; set;} = "";

    // Nome da tag configurada.
    public string TagName {get; set;} = "";

    // Indica se a IA está ativa para esta tag.
    public bool IaAtiva {get; set;} = true;

    // Data e hora em que a configuração foi salva.
    public DateTime DataConfiguracao {get; set;} = DateTime.Now;
}