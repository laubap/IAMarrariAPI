public class BridgeTagsService
{
    private readonly HttpClient _httpClient = new();

    public async Task<string> BuscarTagsCliente(string clienteId)
    {
        var url =
            $"http://localhost:5001/tags?cliente={clienteId}";

        return await _httpClient.GetStringAsync(url);
    }
}