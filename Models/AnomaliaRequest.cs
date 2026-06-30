// Modelo usado para enviar uma requisição de detecção de anomalia.
// Contém o cliente, a tag, o timestamp e o valor a ser analisado.
public class AnomaliaRequest
{
    // Identificador do cliente.
    public string ClienteId { get; set; } = "";

    // Nome da tag que será verificada.
    public string TagName { get; set; } = "";

    // Data e hora em que o valor foi lido ou enviado.
    public DateTime DataHora { get; set; }

    // Valor da tag que será avaliado para detectar anomalia.
    public float Valor { get; set; }
}