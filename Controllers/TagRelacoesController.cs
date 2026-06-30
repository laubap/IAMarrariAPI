using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// Controller para consultar as relações de uma tag com processos e equipamentos.
// Ele busca as conexões da tag nos relacionamentos existentes e retorna
// listas resumidas de processos e equipamentos associados.
[ApiController]
[Route("api/tags")]
public class TagRelacoesController : ControllerBase
{
    private readonly AppDbContext _context;

    public TagRelacoesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{tagName}/relacoes")]
    // Endpoint: GET /api/tags/{tagName}/relacoes
    // Retorna as relações da tag com processos e equipamentos.
    public IActionResult BuscarRelacoes(string tagName)
    {
        var processos = _context.ProcessoTagsIa
            .Include(x => x.ProcessoIa)
            .Where(x => x.TagName == tagName)
            .Select(x => new
            {
                id = x.ProcessoIaId,
                nome = x.ProcessoIa != null ? x.ProcessoIa.Nome : null,
                papelDaTag = x.PapelDaTag
            })
            .ToList();

        var equipamentos = _context.EquipamentoTagsIa
            .Include(x => x.EquipamentoIa)
            .Where(x => x.TagName == tagName)
            .Select(x => new
            {
                id = x.EquipamentoIaId,
                nome = x.EquipamentoIa != null ? x.EquipamentoIa.Nome : null,
                papelDaTag = x.PapelDaTag
            })
            .ToList();

        return Ok(new
        {
            tagName,
            processos,
            equipamentos
        });
    }
}