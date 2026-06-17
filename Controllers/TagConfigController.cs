using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/tags")]
public class TagConfigController : ControllerBase
{
    private readonly AppDbContext _context;

    public TagConfigController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("configurar")]
    public IActionResult ConfigurarTag([FromBody] TagIaConfig config)
    {
        var existente = _context.TagsIaConfig
            .FirstOrDefault(x =>
                x.ClienteId == config.ClienteId &&
                x.TagName == config.TagName);

        if (existente != null)
        {
            existente.TipoTag = config.TipoTag;
            existente.IaAtiva = config.IaAtiva;

            _context.SaveChanges();

            return Ok(new
            {
                mensagem = "Configuração atualizada com sucesso."
            });
        }

        _context.TagsIaConfig.Add(config);
        _context.SaveChanges();

        return Ok(new
        {
            mensagem = "Configuração criada com sucesso."
        });
    }

    [HttpGet]
    public IActionResult Listar()
    {
        return Ok(_context.TagsIaConfig.ToList());
    }

    [HttpGet("{clienteId}/{tagName}")]
    public IActionResult Buscar(string clienteId, string tagName)
    {
        var config = _context.TagsIaConfig
            .FirstOrDefault(x =>
                x.ClienteId == clienteId &&
                x.TagName == tagName);

        if (config == null)
            return NotFound();

        return Ok(config);
    }

    [HttpDelete("{clienteId}/{tagName}")]
    public IActionResult Remover(string clienteId, string tagName)
    {
        var config = _context.TagsIaConfig
            .FirstOrDefault(x =>
                x.ClienteId == clienteId &&
                x.TagName == tagName);

        if (config == null)
            return NotFound();

        _context.TagsIaConfig.Remove(config);
        _context.SaveChanges();

        return Ok(new
        {
            mensagem = "Configuração removida."
        });
    }
}