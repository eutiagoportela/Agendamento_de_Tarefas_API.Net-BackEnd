
using Tarefa.Domain.Enum;

namespace Tarefa.Comunicacao.Responses.Tarefa;

public class TarefaPorPrioridadeResponse
{
    public PrioridadeTarefa Prioridade { get; set; }
    public string PrioridadeTexto => Prioridade.ToString();
    public int Quantidade { get; set; }
    public int Pendentes { get; set; }
    public int Concluidas { get; set; }
}
