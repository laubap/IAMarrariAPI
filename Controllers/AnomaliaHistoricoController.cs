using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/anomalias-historico")]
public class AnomaliaHistoricoController : ControllerBase
{
    private readonly AppDbContext _context;

    public AnomaliaHistoricoController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{clienteId}")]
    public IActionResult ListarPorCliente(string clienteId)
    {
        var historico = _context.AnomaliasDetectadas
            .Include(x => x.Dependencias)
            .Where(x => x.ClienteId == clienteId)
            .OrderByDescending(x => x.DataDeteccao)
            .ToList();

        return Ok(historico);
    }
}