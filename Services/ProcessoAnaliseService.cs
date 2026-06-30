using Microsoft.EntityFrameworkCore;

// Serviço responsável por localizar o processo associado a uma tag.
// Ele consulta a entidade de relacionamento entre tag e processo e retorna
// o processo vinculado, se existir.
public class ProcessoAnaliseService
{
    private readonly AppDbContext _context;

    public ProcessoAnaliseService(AppDbContext context)
    {
        _context = context;
    }

    // Retorna o processo IA associado à tag informada para o cliente.
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