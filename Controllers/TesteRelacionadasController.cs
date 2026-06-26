using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

[ApiController]
[Route("api/teste")]
public class TesteRelacionadasController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly BridgeValorAtualService _bridgeValorAtualService;

    public TesteRelacionadasController(
        AppDbContext context,
        BridgeValorAtualService bridgeValorAtualService)
    {
        _context = context;
        _bridgeValorAtualService = bridgeValorAtualService;
    }

    [HttpGet("valores-relacionados")]
    public async Task<IActionResult> TestarValoresRelacionados(
        string clienteId,
        string tagName)
    {
        var contexto = _context.TagsContextoIa
            .FirstOrDefault(x =>
                x.ClienteId == clienteId &&
                x.TagName == tagName);

        if (contexto == null)
            return NotFound(new { erro = "Contexto da tag não encontrado." });

        var tagsRelacionadas = JsonSerializer.Deserialize<List<string>>(
            contexto.TagsRelacionadas ?? "[]"
        ) ?? new List<string>();

        if (!tagsRelacionadas.Any())
            return BadRequest(new { erro = "Nenhuma tag relacionada cadastrada." });

        var todasAsTags = new List<string> { tagName };
        todasAsTags.AddRange(tagsRelacionadas);

        var valoresAtuais =
            await _bridgeValorAtualService.BuscarValoresAtuais(todasAsTags);

        return Ok(new
        {
            clienteId,
            tagPrincipal = tagName,
            tagsRelacionadas,
            valoresAtuais
        });
    }
}