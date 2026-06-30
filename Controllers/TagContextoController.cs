using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

// Controller para salvar e gerenciar o contexto de uma tag.
// Ele persiste informações de contexto no banco e sincroniza dependências
// relacionadas automaticamente com base nas tags relacionadas.
[ApiController]
[Route("api/tags/contexto")]
public class TagContextoController : ControllerBase
{
    private readonly AppDbContext _context;

    public TagContextoController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    // Endpoint: POST /api/tags/contexto
    // Recebe o contexto da tag e grava ou atualiza o registro em TagsContextoIa.
    // Também atualiza a lista de dependências de tag com base em TagsRelacionadas.
    public IActionResult SalvarContexto([FromBody] TagContextoRequest request)
    {
        var contexto = _context.TagsContextoIa
            .FirstOrDefault(x =>
                x.ClienteId == request.ClienteId &&
                x.TagName == request.TagName);

        if (contexto == null)
        {
            contexto = new TagContextoIa
            {
                ClienteId = request.ClienteId,
                TagName = request.TagName,
                DataCriacao = DateTime.Now
            };

            _context.TagsContextoIa.Add(contexto);
        }

        contexto.TipoRepresentacao = request.TipoRepresentacao;
        contexto.Descricao = request.Descricao;
        contexto.Criticidade = request.Criticidade;
        contexto.Equipamento = request.Equipamento;
        contexto.Area = request.Area;
        contexto.ObservacaoModoOperacao = request.ObservacaoModoOperacao;

        contexto.Impactos = JsonSerializer.Serialize(request.Impactos ?? new List<string>());
        contexto.TagsRelacionadas = JsonSerializer.Serialize(request.TagsRelacionadas ?? new List<string>());
        contexto.ModosOperacao = JsonSerializer.Serialize(request.ModosOperacao ?? new List<string>());
        contexto.CausasProvaveis = JsonSerializer.Serialize(request.CausasProvaveis ?? new List<string>());
        contexto.AcoesRecomendadas = JsonSerializer.Serialize(request.AcoesRecomendadas ?? new List<string>());

        contexto.ContextoCompleto =
            !string.IsNullOrWhiteSpace(request.Descricao) &&
            !string.IsNullOrWhiteSpace(request.Criticidade);

        contexto.DataAtualizacao = DateTime.Now;

        SincronizarDependencias(request);

        _context.SaveChanges();

        return Ok(new
        {
            mensagem = "Contexto da tag salvo com sucesso.",
            contexto.ClienteId,
            contexto.TagName,
            contexto.ContextoCompleto,
            totalDependencias = request.TagsRelacionadas?.Count ?? 0,
            contexto.DataCriacao,
            contexto.DataAtualizacao
        });
    }

    // Sincroniza as dependências de tag no banco de dados com base
    // nas tags relacionadas informadas no contexto da tag.
    private void SincronizarDependencias(TagContextoRequest request)
    {
        var tagsRelacionadas = request.TagsRelacionadas ?? new List<string>();

        tagsRelacionadas = tagsRelacionadas
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var dependenciasAtuais = _context.TagDependenciasIa
            .Where(x =>
                x.ClienteId == request.ClienteId &&
                x.TagName == request.TagName)
            .ToList();

        var dependenciasParaRemover = dependenciasAtuais
            .Where(x => !tagsRelacionadas.Contains(x.TagDependente, StringComparer.OrdinalIgnoreCase))
            .ToList();

        if (dependenciasParaRemover.Any())
            _context.TagDependenciasIa.RemoveRange(dependenciasParaRemover);

        foreach (var tagRelacionada in tagsRelacionadas)
        {
            var existente = dependenciasAtuais
                .FirstOrDefault(x =>
                    string.Equals(x.TagDependente, tagRelacionada, StringComparison.OrdinalIgnoreCase));

            if (existente == null)
            {
                _context.TagDependenciasIa.Add(new TagDependenciaIa
                {
                    ClienteId = request.ClienteId,
                    TagName = request.TagName,
                    TagDependente = tagRelacionada,
                    TipoRelacao = "relacionada_contexto",
                    Impacto = "medio",
                    Descricao = "Dependência cadastrada automaticamente pelo contexto da tag",
                    DataCriacao = DateTime.Now
                });
            }
        }
    }
}

public class TagContextoRequest
{
    // Payload para salvar o contexto de uma tag.
    public string ClienteId { get; set; } = "";
    public string TagName { get; set; } = "";

    public string? TipoRepresentacao { get; set; }
    public string? Descricao { get; set; }
    public string? Criticidade { get; set; }

    public List<string>? Impactos { get; set; }
    public List<string>? TagsRelacionadas { get; set; }
    public List<string>? ModosOperacao { get; set; }
    public List<string>? CausasProvaveis { get; set; }
    public List<string>? AcoesRecomendadas { get; set; }

    public string? Equipamento { get; set; }
    public string? Area { get; set; }
    public string? ObservacaoModoOperacao { get; set; }
}