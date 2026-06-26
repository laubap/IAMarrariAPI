using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/tags/dependencias")]
public class TagDependenciaController : ControllerBase
{
    private readonly AppDbContext _context;

    public TagDependenciaController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult SalvarDependencias([FromBody] SalvarDependenciasRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ClienteId))
            return BadRequest(new { erro = "ClienteId é obrigatório." });

        if (string.IsNullOrWhiteSpace(request.TagName))
            return BadRequest(new { erro = "TagName é obrigatório." });

        if (request.Dependencias == null || !request.Dependencias.Any())
            return BadRequest(new { erro = "Informe ao menos uma dependência." });

        foreach (var dep in request.Dependencias)
        {
            var existente = _context.TagDependenciasIa
                .FirstOrDefault(x =>
                    x.ClienteId == request.ClienteId &&
                    x.TagName == request.TagName &&
                    x.TagDependente == dep.TagDependente);

            if (existente == null)
            {
                existente = new TagDependenciaIa
                {
                    ClienteId = request.ClienteId,
                    TagName = request.TagName,
                    TagDependente = dep.TagDependente
                };

                _context.TagDependenciasIa.Add(existente);
            }

            existente.TipoRelacao = dep.TipoRelacao;
            existente.Impacto = dep.Impacto;
            existente.Descricao = dep.Descricao;
        }

        _context.SaveChanges();

        return Ok(new
        {
            mensagem = "Dependências salvas com sucesso.",
            request.ClienteId,
            request.TagName,
            total = request.Dependencias.Count
        });
    }

    [HttpGet("{clienteId}/{tagName}")]
    public IActionResult ListarDependencias(string clienteId, string tagName)
    {
        var dependencias = _context.TagDependenciasIa
            .Where(x =>
                x.ClienteId == clienteId &&
                x.TagName == tagName)
            .ToList();

        return Ok(dependencias);
    }
}

public class SalvarDependenciasRequest
{
    public string ClienteId { get; set; } = "";
    public string TagName { get; set; } = "";
    public List<DependenciaRequest> Dependencias { get; set; } = new();
}

public class DependenciaRequest
{
    public string TagDependente { get; set; } = "";
    public string TipoRelacao { get; set; } = "";
    public string Impacto { get; set; } = "";
    public string? Descricao { get; set; }
}