using System.Globalization;
using System.Text.Json;

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

            Console.WriteLine("Primeiros registros recebidos:");

            foreach (var item in dados.Take(5))
            {
                Console.WriteLine(
                    $"{item.DataHora:yyyy-MM-dd HH:mm:ss} -> {item.Valor}");
            }

            var resultado = dados
                .Select(x => new LeituraBruta
                {
                    DataHora = x.DataHora,
                    Valor = Convert.ToSingle(
                        x.Valor,
                        CultureInfo.InvariantCulture)
                })
                .ToList();

            Console.WriteLine(
                $"Histórico convertido para IA: {resultado.Count} registros");

            Console.WriteLine("================================");

            return resultado;
        }
        catch (JsonException)
        {
            throw new Exception(
                "O Bridge retornou uma resposta inválida. Verifique o endpoint /historico.");
        }
    }
}

public class BridgeHistoricoItem
{
    public DateTime DataHora { get; set; }
    public string TagName { get; set; } = "";
    public float Valor { get; set; }
}