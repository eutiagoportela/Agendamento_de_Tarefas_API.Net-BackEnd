
using Tarefa.Domain.Enum;

namespace Tarefa.Comunicacao.Requests.Tarefa;

public class ListarTarefasRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public bool? Concluida { get; set; }
    public PrioridadeTarefa? Prioridade { get; set; }
}
