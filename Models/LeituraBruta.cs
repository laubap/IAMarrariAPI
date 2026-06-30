// Modelo simples para representar uma leitura bruta de tag.
// Contém apenas o timestamp e o valor lido.
public class LeituraBruta
{
    // Data e hora em que a leitura foi registrada.
    public DateTime DataHora { get; set; }

    // Valor lido da tag naquele instante.
    public float Valor { get; set; }
}