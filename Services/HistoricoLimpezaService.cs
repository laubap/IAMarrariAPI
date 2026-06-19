// Serviço responsável por limpar o histórico de leituras removendo outliers
// usando o método do IQR (Interquartile Range).
public class HistoricoLimpezaService
{
    // Remove valores considerados outliers do histórico usando o intervalo interquartil (IQR).
    // Entrada: lista de `LeituraBruta` (cada item tem `DataHora` e `Valor`).
    // Retorno: nova lista sem os itens cujo valor esteja fora dos limites calculados pelo IQR.
    public List<LeituraBruta> RemoverOutliersPorIqr(List<LeituraBruta> historico)
    {
        // Se não houver histórico suficiente para calcular percentis (pelo menos 4 valores),
        // retorna o próprio histórico (ou uma lista vazia se for nulo).
        if (historico == null || historico.Count < 4)
            return historico ?? new List<LeituraBruta>();

        // Extrai os valores numéricos, converte para double e ordena em ordem crescente.
        var valoresOrdenados = historico
            .Select(x => Convert.ToDouble(x.Valor))
            .OrderBy(x => x)
            .ToList();

        // Calcula primeiro quartil (Q1) e terceiro quartil (Q3).
        var q1 = CalcularPercentil(valoresOrdenados, 25);
        var q3 = CalcularPercentil(valoresOrdenados, 75);

        // IQR = Q3 - Q1
        var iqr = q3 - q1;

        // Limites inferior e superior para definir outliers.
        // Padrão comum: Q1 - 1.5*IQR  e Q3 + 1.5*IQR
        var limiteInferior = q1 - (1.5 * iqr);
        var limiteSuperior = q3 + (1.5 * iqr);

        // Logs para depuração / análise rápida no console.
        Console.WriteLine("================================");
        Console.WriteLine("LIMPEZA DE HISTÓRICO POR IQR");
        Console.WriteLine($"Total original: {historico.Count}");
        Console.WriteLine($"Q1: {q1}");
        Console.WriteLine($"Q3: {q3}");
        Console.WriteLine($"IQR: {iqr}");
        Console.WriteLine($"Limite inferior: {limiteInferior}");
        Console.WriteLine($"Limite superior: {limiteSuperior}");

        // Filtra os itens que estão dentro dos limites.
        var historicoLimpo = historico
            .Where(x =>
            {
                var valor = Convert.ToDouble(x.Valor);
                // Mantém apenas valores entre limiteInferior e limiteSuperior (inclusive).
                return valor >= limiteInferior && valor <= limiteSuperior;
            })
            .ToList();

        Console.WriteLine($"Total após limpeza: {historicoLimpo.Count}");
        Console.WriteLine($"Removidos: {historico.Count - historicoLimpo.Count}");
        Console.WriteLine("================================");

        return historicoLimpo;
    }

    // Calcula o percentil de uma lista já ordenada de valores.
    // Implementa interpolação linear entre posições quando o percentil não cair
    // exatamente sobre um índice inteiro.
    private double CalcularPercentil(List<double> valoresOrdenados, double percentil)
    {
        // Proteção contra entrada inválida.
        if (valoresOrdenados == null || valoresOrdenados.Count == 0)
            return 0;

        // Posição contínua do percentil na lista (0-based):
        // posicao = p/100 * (N-1)
        var posicao = (percentil / 100.0) * (valoresOrdenados.Count - 1);
        var indiceInferior = (int)Math.Floor(posicao);
        var indiceSuperior = (int)Math.Ceiling(posicao);

        // Se a posição for exata, retorna o valor correspondente.
        if (indiceInferior == indiceSuperior)
            return valoresOrdenados[indiceInferior];

        // Caso contrário, faz interpolação linear entre os dois valores.
        var peso = posicao - indiceInferior;

        return valoresOrdenados[indiceInferior] +
               (valoresOrdenados[indiceSuperior] - valoresOrdenados[indiceInferior]) * peso;
    }
}