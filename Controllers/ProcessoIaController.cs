using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/processos")]
public class ProcessoIaController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProcessoIaController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult Salvar([FromBody] ProcessoIaRequest request)
    {
        var processo = _context.ProcessosIa
            .Include(x => x.Tags)
            .FirstOrDefault(x =>
                x.ClienteId == request.ClienteId &&
                x.Nome == request.Nome);

        if (processo == null)
        {
            processo = new ProcessoIa
            {
                ClienteId = request.ClienteId,
                Nome = request.Nome,
                DataCriacao = DateTime.Now
            };

            _context.ProcessosIa.Add(processo);
        }

        processo.Descricao = request.Descricao;
        processo.Area = request.Area;
        processo.Criticidade = request.Criticidade;
        processo.Objetivo = request.Objetivo;
        processo.CondicaoNormal = request.CondicaoNormal;
        processo.ConsequenciasFalha = request.ConsequenciasFalha;
        processo.ProcedimentoRecomendado = request.ProcedimentoRecomendado;
        processo.DataAtualizacao = DateTime.Now;

        _context.SaveChanges();

        var tagsAtuais = _context.ProcessoTagsIa
            .Where(x => x.ProcessoIaId == processo.Id)
            .ToList();

        _context.ProcessoTagsIa.RemoveRange(tagsAtuais);

        foreach (var tag in request.Tags ?? new List<ProcessoTagRequest>())
        {
            _context.ProcessoTagsIa.Add(new ProcessoTagIa
            {
                ProcessoIaId = processo.Id,
                ClienteId = request.ClienteId,
                TagName = tag.TagName,
                PapelDaTag = tag.PapelDaTag
            });
        }

        _context.SaveChanges();

        return Ok(new
        {
            mensagem = "Processo salvo com sucesso.",
            processo.Id,
            processo.ClienteId,
            processo.Nome,
            totalTags = request.Tags?.Count ?? 0
        });
    }

    [HttpGet("{clienteId}")]
    public IActionResult ListarPorCliente(string clienteId)
    {
        var processos = _context.ProcessosIa
            .Include(x => x.Tags)
            .Where(x => x.ClienteId == clienteId)
            .ToList();

        return Ok(processos);
    }
}

public class ProcessoIaRequest
{
    public string ClienteId { get; set; } = "";
    public string Nome { get; set; } = "";

    public string? Descricao { get; set; }
    public string? Area { get; set; }
    public string? Criticidade { get; set; }
    public string? Objetivo { get; set; }
    public string? CondicaoNormal { get; set; }
    public string? ConsequenciasFalha { get; set; }
    public string? ProcedimentoRecomendado { get; set; }

    public List<ProcessoTagRequest>? Tags { get; set; }
}

public class ProcessoTagRequest
{
    public string TagName { get; set; } = "";
    public string? PapelDaTag { get; set; }
}