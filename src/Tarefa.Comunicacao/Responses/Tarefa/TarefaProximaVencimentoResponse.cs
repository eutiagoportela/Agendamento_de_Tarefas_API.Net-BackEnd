
namespace Tarefa.Comunicacao.Responses.Tarefa;

public class TarefaProximaVencimentoResponse
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public DateTime DataConclusao { get; set; }
    public int DiasRestantes { get; set; }
    public bool EstaAtrasada { get; set; }
}
