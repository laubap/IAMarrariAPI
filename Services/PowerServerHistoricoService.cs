using Marrari.PowerServer;

public class PowerServerHistoricoService : IHistoricoService
{
    public List<LeituraBruta> BuscarHistorico(string tagName, string tipoTag)
    {
        using var psClient = new ClientEx(
            "10.0.1.99",
            "admin",
            "admin"
        );

        if (!psClient.ReConnect())
            throw new Exception(
                $"Falha ao conectar: {psClient.LastException?.Message}",
                psClient.LastException
            );

        Console.WriteLine("Conectou com sucesso!");

        var inicio = DateTime.Now.AddMinutes(-10);
        var fim = DateTime.Now;

        var records = psClient.Service.SelectiveMinutesRangeGet(
            new[] { tagName },
            inicio,
            fim
        );

        return records
            .Where(item =>
                item != null &&
                item.Tags != null &&
                item.Tags.Count > 0 &&
                item.Tags[0] != null &&
                item.Tags[0].Value != null
            )
            .Select(item => new LeituraBruta
            {
                DataHora = item.TimeStamp,
                Valor = Convert.ToSingle(item.Tags[0].Value)
            })
            .ToList();
    }
}