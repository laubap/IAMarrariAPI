using System.Globalization;
using System.Text.Json;

// Serviço responsável por buscar o histórico de leituras de uma tag no Bridge externo.
// Ele prepara a URL de consulta, faz a requisição HTTP e converte a resposta JSON
// em uma lista de objetos LeituraBruta para uso pela IA.
public class BridgeHistoricoService : IHistoricoService
{
    private readonly HttpClient _httpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(15)
    };

    private readonly IConfiguration _configuration;

    public BridgeHistoricoService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Busca histórico de valores para a tag informada.
    // O parâmetro tipoTag não é utilizado internamente aqui, mas a assinatura mantém
    // compatibilidade com a interface IHistoricoService.
    public List<LeituraBruta> BuscarHistorico(string tagName, string tipoTag)
    {
        var baseUrl =
            _configuration["Bridge:BaseUrl"]
            ?? "http://localhost:5001";

        var horas =
            _configuration["Bridge:HorasHistorico"]
            ?? "24";

        var url =
            $"{baseUrl}/historico?tagName={Uri.EscapeDataString(tagName)}&horas={horas}";

        Console.WriteLine("================================");
        Console.WriteLine("BUSCANDO HISTÓRICO NO BRIDGE");
        Console.WriteLine("Tag: " + tagName);
        Console.WriteLine("Horas configuradas: " + horas);
        Console.WriteLine("URL: " + url);
        Console.WriteLine("================================");

        string json;

        try
        {
            json = _httpClient.GetStringAsync(url).Result;
        }
        catch (AggregateException ex) when (ex.InnerException is HttpRequestException)
        {
            throw new Exception(
                $"Não foi possível conectar ao Bridge ({baseUrl}). Verifique se ele está rodando.");
        }
        catch (AggregateException ex) when (ex.InnerException is TaskCanceledException)
        {
            throw new Exception(
                "Tempo limite excedido ao consultar o Bridge.");
        }
        catch (HttpRequestException)
        {
            throw new Exception(
                "Erro ao consultar o Bridge/PowerServer.");
        }

        try
        {
            var dados = JsonSerializer.Deserialize<List<BridgeHistoricoItem>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (dados == null || dados.Count == 0)
            {
                throw new Exception(
                    $"Nenhum histórico encontrado para a tag '{tagName}'.");
            }

            Console.WriteLine($"Histórico recebido: {dados.Count} registros");

            var resultado = new List<LeituraBruta>();

            foreach (var item in dados)
            {
                float valor;

                if (item.Valor.ValueKind == JsonValueKind.Number)
                {
                    valor = item.Valor.GetSingle();
                }
                else if (item.Valor.ValueKind == JsonValueKind.String)
                {
                    var texto = item.Valor.GetString();

                    if (string.IsNullOrWhiteSpace(texto))
                        valor = float.NaN;
                    else if (texto.Equals("NaN", StringComparison.OrdinalIgnoreCase))
                        valor = float.NaN;
                    else if (!float.TryParse(
                                texto,
                                NumberStyles.Float,
                                CultureInfo.InvariantCulture,
                                out valor))
                    {
                        valor = float.NaN;
                    }
                }
                else
                {
                    valor = float.NaN;
                }

                resultado.Add(new LeituraBruta
                {
                    DataHora = item.DataHora,
                    Valor = valor
                });
            }

            Console.WriteLine(
                $"Histórico convertido para IA: {resultado.Count} registros");

            Console.WriteLine("================================");

            return resultado;
        }
        catch (JsonException ex)
        {
            throw new Exception(
                $"O Bridge retornou uma resposta inválida. Erro JSON: {ex.Message}. Resposta recebida: {json}");
        }
    }
}

// Modelo que representa cada item retornado pelo Bridge para histórico de tag.
// A propriedade Valor é mantida como JsonElement para permitir conversão
// segura a partir de números ou strings no JSON.
public class BridgeHistoricoItem
{
    public DateTime DataHora { get; set; }

    public string TagName { get; set; } = "";

    public JsonElement Valor { get; set; }
}