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

    [HttpGet("perfis")]
    public IActionResult ListarPerfis()
    {
        var perfis = _context.PerfisIa
            .OrderBy(x => x.ClienteId)
            .ThenBy(x => x.TagName)
            .Select(x => new
            {
                x.Id,
                x.ClienteId,
                x.TagName,
                x.Media,
                x.DesvioPadrao,
                x.Minimo,
                x.Maximo,
                x.Amplitude,
                x.PercentualZeros,
                x.VariacaoMedia,
                x.QuantidadePicos,
                x.TotalRegistrosHistorico,
                x.TotalRegistrosUsados,
                x.TotalOutliersRemovidos,
                x.DataTreinamento
            })
            .ToList();

        return Ok(perfis);
    }


    [HttpPost("retreinar-perfil")]
public IActionResult RetreinarPerfil([FromBody] TreinarPerfilRequest request)
{
    var perfil = _perfilTagService.TreinarPerfil(
        request.ClienteId,
        request.TagName
    );

    return Ok(new
    {
        mensagem = "Perfil retreinado com sucesso.",
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
        perfil.TotalRegistrosHistorico,
        perfil.TotalRegistrosUsados,
        perfil.TotalOutliersRemovidos,
        perfil.DataTreinamento
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

