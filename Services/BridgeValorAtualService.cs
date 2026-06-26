using System.Globalization;
using System.Text;
using System.Text.Json;

public class BridgeValorAtualService
{
    private readonly HttpClient _httpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(60)
    };

    public async Task<List<ValorAtualTagResponse>> BuscarValoresAtuais(
        List<string> tags)
    {
        var request = new
        {
            Tags = tags
        };

        var json = JsonSerializer.Serialize(request);

        var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync(
            "http://localhost:5001/valores-atuais",
            content
        );

        response.EnsureSuccessStatusCode();

        var responseJson =
            await response.Content.ReadAsStringAsync();

        var dados = JsonSerializer.Deserialize<List<ValorAtualTagBridgeItem>>(
            responseJson,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        ) ?? new List<ValorAtualTagBridgeItem>();

        return dados.Select(x => new ValorAtualTagResponse
        {
            TagName = x.TagName,
            Valor = ConverterValor(x.Valor),
            DataHora = x.DataHora,
            Unidade = x.Unidade,
            Descricao = x.Descricao,
            Encontrado = x.Encontrado && !double.IsNaN(ConverterValor(x.Valor))
        }).ToList();
    }

    private double ConverterValor(JsonElement valor)
    {
        if (valor.ValueKind == JsonValueKind.Number)
            return valor.GetDouble();

        if (valor.ValueKind == JsonValueKind.String)
        {
            var texto = valor.GetString();

            if (string.IsNullOrWhiteSpace(texto))
                return double.NaN;

            if (texto.Equals("NaN", StringComparison.OrdinalIgnoreCase))
                return double.NaN;

            if (double.TryParse(
                    texto,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out var numero))
            {
                return numero;
            }
        }

        return double.NaN;
    }
}

public class ValorAtualTagBridgeItem
{
    public string TagName { get; set; } = "";

    public JsonElement Valor { get; set; }

    public DateTime DataHora { get; set; }

    public string? Unidade { get; set; }

    public string? Descricao { get; set; }

    public bool Encontrado { get; set; }
}

public class ValorAtualTagResponse
{
    public string TagName { get; set; } = "";

    public double Valor { get; set; }

    public DateTime DataHora { get; set; }

    public string? Unidade { get; set; }

    public string? Descricao { get; set; }

    public bool Encontrado { get; set; }
}