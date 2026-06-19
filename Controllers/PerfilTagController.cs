using Microsoft.AspNetCore.Mvc;
using System.Linq;

[ApiController]
[Route("api/tags")]
public class PerfilTagController : ControllerBase
{
    private readonly PerfilTagService _perfilTagService;
    private readonly AppDbContext _context;

    public PerfilTagController(
        PerfilTagService perfilTagService,
        AppDbContext context)
    {
        _perfilTagService = perfilTagService;
        _context = context;
    }

    [HttpPost("treinar-perfil")]
    public IActionResult TreinarPerfil([FromBody] TreinarPerfilRequest request)
    {
        var perfil = _perfilTagService.TreinarPerfil(
            request.ClienteId,
            request.TagName
        );

        return Ok(new
        {
            mensagem = "Perfil treinado com sucesso.",

            perfil.ClienteId,
            perfil.TagName,

            perfil.Media,
            perfil.DesvioPadrao,

            perfil.Minimo,
            perfil.Maximo,

            perfil.Amplitude,
            perfil.PercentualZeros,
            perfil.VariacaoMedia,
            perfil.QuantidadePicos,

            perfil.DataTreinamento
        });
    }

    [HttpPost("testar-perfil")]
    public IActionResult TestarPerfil([FromBody] TestarPerfilRequest request)
    {
        var perfil = _context.PerfisIa
            .FirstOrDefault(x =>
                x.ClienteId == request.ClienteId &&
                x.TagName == request.TagName);

        if (perfil == null)
        {
            return NotFound(new
            {
                erro = "Perfil da tag não encontrado. Treine a tag primeiro."
            });
        }

        var limiteInferior =
            perfil.Media - (3 * perfil.DesvioPadrao);

        var limiteSuperior =
            perfil.Media + (3 * perfil.DesvioPadrao);

        var ehAnomalia =
            request.Valor < limiteInferior ||
            request.Valor > limiteSuperior;

        return Ok(new
        {
            request.ClienteId,
            request.TagName,
            valorAtual = request.Valor,

            perfil.Media,
            perfil.DesvioPadrao,

            perfil.Minimo,
            perfil.Maximo,

            perfil.Amplitude,
            perfil.PercentualZeros,
            perfil.VariacaoMedia,
            perfil.QuantidadePicos,

            limiteInferior,
            limiteSuperior,

            ehAnomalia,

            mensagem = ehAnomalia
                ? "Anômalia detectada. Valor fora do comportamento esperado da tag."
                : "Valor dentro do comportamento esperado da tag."
        });
    }
}

public class TreinarPerfilRequest
{
    public string ClienteId { get; set; } = "";
    public string TagName { get; set; } = "";
}

public class TestarPerfilRequest
{
    public string ClienteId { get; set; } = "";
    public string TagName { get; set; } = "";
    public double Valor { get; set; }
}

