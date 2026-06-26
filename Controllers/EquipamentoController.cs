using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/equipamentos")]
public class EquipamentoController : ControllerBase
{
    private readonly AppDbContext _context;

    public EquipamentoController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult Salvar([FromBody] EquipamentoRequest request)
    {
        var equipamento = new EquipamentoIa
        {
            ClienteId = request.ClienteId,
            Nome = request.Nome,
            Descricao = request.Descricao,
            Area = request.Area,
            TipoEquipamento = request.TipoEquipamento,
            Criticidade = request.Criticidade,
            Fabricante = request.Fabricante,
            Modelo = request.Modelo,
            ProcessoIaId = request.ProcessoIaId,
            Observacoes = request.Observacoes,
            DataUltimaManutencao = request.DataUltimaManutencao,
            DataCriacao = DateTime.Now,
            DataAtualizacao = DateTime.Now
        };

        _context.EquipamentosIa.Add(equipamento);
        _context.SaveChanges();

        return Ok(new
        {
            mensagem = "Equipamento cadastrado com sucesso.",
            equipamento.Id,
            equipamento.Nome
        });
    }

    [HttpGet]
    public IActionResult Listar()
    {
        return Ok(
            _context.EquipamentosIa
                .Include(x => x.Tags)
                .ToList()
        );
    }

    [HttpPost("{equipamentoId}/tags")]
    public IActionResult VincularTag(
        int equipamentoId,
        [FromBody] EquipamentoTagRequest request)
    {
        var equipamento = _context.EquipamentosIa
            .FirstOrDefault(x => x.Id == equipamentoId);

        if (equipamento == null)
        {
            return NotFound(new
            {
                erro = "Equipamento não encontrado."
            });
        }

        var existente = _context.EquipamentoTagsIa
            .FirstOrDefault(x =>
                x.EquipamentoIaId == equipamentoId &&
                x.TagName == request.TagName);

        if (existente != null)
        {
            existente.PapelDaTag = request.PapelDaTag;

            _context.SaveChanges();

            return Ok(new
            {
                mensagem = "Tag do equipamento atualizada com sucesso.",
                equipamentoId,
                request.TagName,
                request.PapelDaTag
            });
        }

        var tag = new EquipamentoTagIa
        {
            EquipamentoIaId = equipamentoId,
            ClienteId = request.ClienteId,
            TagName = request.TagName,
            PapelDaTag = request.PapelDaTag
        };

        _context.EquipamentoTagsIa.Add(tag);
        _context.SaveChanges();

        return Ok(new
        {
            mensagem = "Tag vinculada ao equipamento com sucesso.",
            equipamentoId,
            tag.TagName,
            tag.PapelDaTag
        });
    }
}

public class EquipamentoRequest
{
    public string ClienteId { get; set; } = "";
    public string Nome { get; set; } = "";
    public string? Descricao { get; set; }
    public string? Area { get; set; }
    public string? TipoEquipamento { get; set; }
    public string? Criticidade { get; set; }
    public string? Fabricante { get; set; }
    public string? Modelo { get; set; }
    public string? Observacoes { get; set; }
    public DateTime? DataUltimaManutencao { get; set; }
    public int? ProcessoIaId { get; set; }
}

public class EquipamentoTagRequest
{
    public string ClienteId { get; set; } = "";
    public string TagName { get; set; } = "";
    public string? PapelDaTag { get; set; }
}