using Microsoft.EntityFrameworkCore;

public class ProcessoAnaliseService
{
    private readonly AppDbContext _context;

    public ProcessoAnaliseService(AppDbContext context)
    {
        _context = context;
    }

    public ProcessoIa? BuscarProcessoDaTag(string clienteId, string tagName)
    {
        var processoTag = _context.ProcessoTagsIa
            .Include(x => x.ProcessoIa)
            .FirstOrDefault(x =>
                x.ClienteId == clienteId &&
                x.TagName == tagName);

        return processoTag?.ProcessoIa;
    }
}