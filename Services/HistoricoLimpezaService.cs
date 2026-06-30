// Serviço que limpa séries históricas de leituras, removendo valores inválidos
// e outliers usando o método do intervalo interquartil (IQR).
public class HistoricoLimpezaService
{
    // Remove NaN e Infinity e, em seguida, aplica limpeza por IQR para excluir
    // valores extremos. Retorna o histórico filtrado para uso em análises.
    public List<LeituraBruta> RemoverOutliersPorIqr(List<LeituraBruta> historico)
    {
        if (historico == null || historico.Count == 0)
            return new List<LeituraBruta>();

        var historicoSemInvalidos = historico
            .Where(x =>
                !float.IsNaN(x.Valor) &&
                !float.IsInfinity(x.Valor))
            .ToList();

        Console.WriteLine("================================");
        Console.WriteLine("LIMPEZA DE HISTÓRICO - VALORES INVÁLIDOS");
        Console.WriteLine($"Total original: {historico.Count}");
        Console.WriteLine($"Total após remover NaN/Infinity: {historicoSemInvalidos.Count}");
        Console.WriteLine($"Inválidos removidos: {historico.Count - historicoSemInvalidos.Count}");
        Console.WriteLine("================================");

        if (historicoSemInvalidos.Count < 4)
            return historicoSemInvalidos;

        var valoresOrdenados = historicoSemInvalidos
            .Select(x => Convert.ToDouble(x.Valor))
            .OrderBy(x => x)
            .ToList();

        var q1 = CalcularPercentil(valoresOrdenados, 25);
        var q3 = CalcularPercentil(valoresOrdenados, 75);
        var iqr = q3 - q1;

        var limiteInferior = q1 - (1.5 * iqr);
        var limiteSuperior = q3 + (1.5 * iqr);

        Console.WriteLine("================================");
        Console.WriteLine("LIMPEZA DE HISTÓRICO POR IQR");
        Console.WriteLine($"Total antes do IQR: {historicoSemInvalidos.Count}");
        Console.WriteLine($"Q1: {q1}");
        Console.WriteLine($"Q3: {q3}");
        Console.WriteLine($"IQR: {iqr}");
        Console.WriteLine($"Limite inferior: {limiteInferior}");
        Console.WriteLine($"Limite superior: {limiteSuperior}");

        var historicoLimpo = historicoSemInvalidos
            .Where(x =>
            {
                var valor = Convert.ToDouble(x.Valor);
                return valor >= limiteInferior && valor <= limiteSuperior;
            })
            .ToList();

        Console.WriteLine($"Total após IQR: {historicoLimpo.Count}");
        Console.WriteLine($"Outliers removidos: {historicoSemInvalidos.Count - historicoLimpo.Count}");
        Console.WriteLine("================================");

        return historicoLimpo;
    }

    // Calcula o percentil de uma lista ordenada de valores usando interpolação.
    private double CalcularPercentil(List<double> valoresOrdenados, double percentil)
    {
        if (valoresOrdenados == null || valoresOrdenados.Count == 0)
            return 0;

        var posicao = (percentil / 100.0) * (valoresOrdenados.Count - 1);
        var indiceInferior = (int)Math.Floor(posicao);
        var indiceSuperior = (int)Math.Ceiling(posicao);

        if (indiceInferior == indiceSuperior)
            return valoresOrdenados[indiceInferior];

        var peso = posicao - indiceInferior;

        return valoresOrdenados[indiceInferior] +
               (valoresOrdenados[indiceSuperior] - valoresOrdenados[indiceInferior]) * peso;
    }
}