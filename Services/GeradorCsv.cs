using System.Globalization;

public static class CsvFeatureGenerator
{ 
    //PROVAVELMENTE ESSE FILE N VAI SER USADA, MAS A LOGICA DENTRO DELE SIM
    public static void Gerar(string caminhoEntrada, string caminhoSaida)
    {
        var historico = CarregarHistorico(caminhoEntrada);

        var linhasSaida = new List<string>
        {
            "DataHora;ValorAtual;MediaMovel5;Variacao;MinJanela5;MaxJanela5;DesvioPadrao5"
        };

        for (int i = 0; i < historico.Count; i++)
        {
            var features = GerarFeatures(historico, i);

            linhasSaida.Add(
                $"{historico[i].DataHora:dd/MM/yyyy HH:mm};" +
                $"{features.ValorAtual.ToString(CultureInfo.InvariantCulture)};" +
                $"{features.MediaMovel5.ToString("F4", CultureInfo.InvariantCulture)};" +
                $"{features.Variacao.ToString("F4", CultureInfo.InvariantCulture)};" +
                $"{features.MinJanela5.ToString(CultureInfo.InvariantCulture)};" +
                $"{features.MaxJanela5.ToString(CultureInfo.InvariantCulture)};" +
                $"{features.DesvioPadrao5.ToString("F4", CultureInfo.InvariantCulture)}"
            );
        }

        File.WriteAllLines(caminhoSaida, linhasSaida);
        Console.WriteLine("CSV enriquecido gerado com sucesso!");
    }

    public static List<LeituraBruta> CarregarHistorico(string caminho)
    {
        return File.ReadAllLines(caminho)
            .Skip(1)
            .Select(linha =>
            {
                var partes = linha.Split(';');

                return new LeituraBruta
                {
                    DataHora = DateTime.ParseExact(
                        partes[0],
                        "dd/MM/yyyy HH:mm",
                        CultureInfo.InvariantCulture
                    ),
                    Valor = float.Parse(partes[1], CultureInfo.InvariantCulture)
                };
            })
            .ToList();
    }

    public static SensorData GerarFeaturesParaNovaLeitura(
        List<LeituraBruta> historico,
        DateTime dataHora,
        float valor
    )
    {
        historico.Add(new LeituraBruta
        {
            DataHora = dataHora,
            Valor = valor
        });

        return GerarFeatures(historico, historico.Count - 1);
    }

    private static SensorData GerarFeatures(List<LeituraBruta> historico, int index)
    {
        var valorAtual = historico[index].Valor;

        var inicioJanela = Math.Max(0, index - 4);

        var janela = historico
            .Skip(inicioJanela)
            .Take(index - inicioJanela + 1)
            .Select(x => x.Valor)
            .ToList();

        var media = janela.Average();

        var variacao = index == 0
            ? 0
            : valorAtual - historico[index - 1].Valor;

        return new SensorData
        {
            ValorAtual = valorAtual,
            MediaMovel5 = media,
            Variacao = variacao,
            MinJanela5 = janela.Min(),
            MaxJanela5 = janela.Max(),
            DesvioPadrao5 = CalcularDesvioPadrao(janela)
        };
    }

    private static float CalcularDesvioPadrao(List<float> valores)
    {
        if (valores.Count <= 1)
            return 0;

        var media = valores.Average();
        var somaQuadrados = valores.Sum(v => Math.Pow(v - media, 2));

        return (float)Math.Sqrt(somaQuadrados / valores.Count);
    }
}