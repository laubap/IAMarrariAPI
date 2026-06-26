public class AnomaliaHistoricoService
{
    private readonly AppDbContext _context;

    public AnomaliaHistoricoService(AppDbContext context)
    {
        _context = context;
    }

    public void Registrar(
        string clienteId,
        string tagName,
        string tipoTag,
        double valor,
        double score,
        bool ehAnomalia,
        string mensagem,
        string? criticidade,
        AnaliseRelacionadasResultado? analiseRelacionadas,
        int scoreRiscoProcesso,
        string? classificacaoRisco,
        string? tendenciaRisco,
        string? tendenciaValor)
    {
        var anomalia = new AnomaliaDetectada
        {
            ClienteId = clienteId,
            TagName = tagName,
            TipoTag = tipoTag,
            ValorRecebido = valor,
            Score = score,
            EhAnomalia = ehAnomalia,
            Mensagem = mensagem,
            Criticidade = criticidade,
            ScoreRiscoProcesso = scoreRiscoProcesso,
            ClassificacaoRisco = classificacaoRisco,
            TendenciaRisco = tendenciaRisco,
            TendenciaValor = tendenciaValor,
            DataDeteccao = DateTime.Now
        };

        _context.AnomaliasDetectadas.Add(anomalia);
        _context.SaveChanges();

        if (analiseRelacionadas == null || !analiseRelacionadas.Resultados.Any())
            return;

        foreach (var dep in analiseRelacionadas.Resultados)
        {
            var dependencia = new AnomaliaDependenciaDetectada
            {
                AnomaliaDetectadaId = anomalia.Id,
                TagDependente = dep.TagName,
                ValorAtual = dep.ValorAtual,
                Media = dep.Media,
                LimiteInferior = dep.LimiteInferior,
                LimiteSuperior = dep.LimiteSuperior,
                EhAnomalia = dep.EhAnomalia,
                Status = dep.EhAnomalia ? "anomala" : "normal",
                TipoRelacao = dep.TipoRelacao,
                Impacto = dep.Impacto,
                DescricaoRelacao = dep.DescricaoRelacao,
                DataDeteccao = DateTime.Now
            };

            _context.AnomaliaDependenciasDetectadas.Add(dependencia);
        }

        _context.SaveChanges();
    }
}