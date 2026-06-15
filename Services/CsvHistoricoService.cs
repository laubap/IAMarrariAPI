public class CsvHistoricoService : IHistoricoService
{
    public List<LeituraBruta> BuscarHistorico(string tagName)
    {
        // Por enquanto ignora o tagName e usa um CSV fake/local
        return CsvFeatureGenerator.CarregarHistorico("Data/sensores.csv");
    }
}