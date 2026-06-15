public class CsvHistoricoService : IHistoricoService
{
    public List<LeituraBruta> BuscarHistorico(string tagName, string tipoTag)
{
    var caminhoCsv = tipoTag.ToLower() switch
    {
        "temperatura" => "Data/temperatura.csv",
        "pressao" => "Data/pressao.csv",
        "corrente" => "Data/corrente.csv",
        _ => throw new Exception($"Tipo de tag inválido: {tipoTag}")
    };

    return CsvFeatureGenerator.CarregarHistorico(caminhoCsv);
}
}

