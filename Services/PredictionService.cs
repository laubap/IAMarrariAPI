public class PredictionService
{
    private readonly AppDbContext _context;

    public PredictionService(AppDbContext context)
    {
        _context = context;
    }

    public AnomaliaResponse Detectar(AnomaliaRequest request)
    {
        var config = _context.TagsIaConfig
            .FirstOrDefault(x =>
                x.ClienteId == request.ClienteId &&
                x.TagName == request.TagName &&
                x.IaAtiva);

        if (config == null)
            throw new Exception($"IA não configurada para a tag {request.TagName} do cliente {request.ClienteId}.");

        var perfil = _context.PerfisIa
            .FirstOrDefault(x =>
                x.ClienteId == request.ClienteId &&
                x.TagName == request.TagName);

        if (perfil == null)
            throw new Exception($"Perfil da tag {request.TagName} ainda não foi treinado. Execute o treinamento de perfil antes da detecção.");

        var limiteInferior = perfil.Media - (3 * perfil.DesvioPadrao);
        var limiteSuperior = perfil.Media + (3 * perfil.DesvioPadrao);

        var ehAnomalia =
            request.Valor < limiteInferior ||
            request.Valor > limiteSuperior;

        var distanciaDaMedia = Math.Abs(request.Valor - perfil.Media);

        var score = perfil.DesvioPadrao > 0
            ? distanciaDaMedia / perfil.DesvioPadrao
            : 0;

        return new AnomaliaResponse
        {
            ClienteId = request.ClienteId,
            TagName = request.TagName,

            // Como agora não dependemos mais de temperatura/pressão/corrente,
            // usamos um tipo genérico para representar a nova arquitetura.
            TipoTag = "perfil-comportamental",

            EhAnomalia = ehAnomalia,
            Score = (float)score,

            Mensagem = ehAnomalia
                ? $"Anomalia detectada na tag {request.TagName}. Valor fora do perfil comportamental esperado."
                : $"Comportamento normal na tag {request.TagName}. Valor dentro do perfil esperado."
        };
    }
}