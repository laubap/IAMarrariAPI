using Microsoft.AspNetCore.Mvc;

// Controller para gerenciar configurações de IA por tag.
// Ele permite criar, atualizar, listar, buscar e remover configurações
// de ativação de IA para uma tag específica de um cliente.
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
    // Endpoint: POST /api/tags/configurar
    // Cria ou atualiza a configuração de IA para a tag informada.
    public IActionResult ConfigurarTag([FromBody] TagIaConfig config)
    {
        var existente = _context.TagsIaConfig
            .FirstOrDefault(x =>
                x.ClienteId == config.ClienteId &&
                x.TagName == config.TagName);

        if (existente != null)
        {
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
    // Endpoint: GET /api/tags
    // Retorna todas as configurações de IA de tags.
    public IActionResult Listar()
    {
        return Ok(_context.TagsIaConfig.ToList());
    }

    [HttpGet("{clienteId}/{tagName}")]
    // Endpoint: GET /api/tags/{clienteId}/{tagName}
    // Retorna a configuração de IA para a tag específica do cliente.
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
    // Endpoint: DELETE /api/tags/{clienteId}/{tagName}
    // Remove a configuração de IA da tag especificada.
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