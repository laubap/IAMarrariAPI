// Interface que define um contrato para serviços de histórico de tags.
// Qualquer implementação deve fornecer um método para buscar o histórico de
// leituras de uma tag e retornar uma lista de LeituraBruta.
public interface IHistoricoService
{
    // Retorna o histórico bruto de leituras para a tag informada.
    List<LeituraBruta> BuscarHistorico(string tagName, string TipoTag);
}