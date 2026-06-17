using System.Globalization;
using System.Text.Json;

public class BridgeHistoricoService : IHistoricoService
{
    private readonly HttpClient _httpClient = new();

    public List<LeituraBruta> BuscarHistorico(string tagName, string tipoTag)
    {
        var url = $"http://localhost:5001/historico?tagName={Uri.EscapeDataString(tagName)}&horas=2";

        Console.WriteLine("================================");
        Console.WriteLine("BUSCANDO HISTÓRICO NO BRIDGE");
        Console.WriteLine("Tag: " + tagName);
        Console.WriteLine("TipoTag: " + tipoTag);
        Console.WriteLine("URL: " + url);
        Console.WriteLine("================================");

        var json = _httpClient.GetStringAsync(url).Result;

        Console.WriteLine("Resposta bruta do Bridge:");
        Console.WriteLine(json);

        var dados = JsonSerializer.Deserialize<List<BridgeHistoricoItem>>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        );

        Console.WriteLine($"Histórico recebido: {dados?.Count ?? 0} registros");

        if (dados != null && dados.Any())
        {
            Console.WriteLine("Primeiros registros recebidos:");

            foreach (var item in dados.Take(5))
            {
                Console.WriteLine($"{item.DataHora:yyyy-MM-dd HH:mm:ss} -> {item.Valor}");
            }
        }

        var historico = dados?
            .Select(x => new LeituraBruta
            {
                DataHora = x.DataHora,
                Valor = Convert.ToSingle(x.Valor, CultureInfo.InvariantCulture)
            })
            .ToList()
            ?? new List<LeituraBruta>();

        Console.WriteLine($"Histórico convertido para IA: {historico.Count} registros");
        Console.WriteLine("================================");

        return historico;
    }
}

public class BridgeHistoricoItem
{
    public DateTime DataHora { get; set; }
    public string TagName { get; set; } = "";
    public float Valor { get; set; }
}