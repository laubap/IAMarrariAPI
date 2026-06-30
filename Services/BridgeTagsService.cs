// Serviço responsável por consultar as tags de um cliente no Bridge externo.
// Ele faz uma requisição HTTP GET para o endpoint /tags e retorna o JSON cru.
public class BridgeTagsService
{
    private readonly HttpClient _httpClient = new();

    // Retorna as tags do cliente informado como string JSON.
    public async Task<string> BuscarTagsCliente(string clienteId)
    {
        var url =
            $"http://localhost:5001/tags?cliente={clienteId}";

        return await _httpClient.GetStringAsync(url);
    }
}