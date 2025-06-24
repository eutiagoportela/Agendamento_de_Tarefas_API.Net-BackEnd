
using System.ComponentModel.DataAnnotations;
using Tarefa.Domain.Enum;


namespace Tarefa.Comunicacao.Requests;

public class AtualizarTarefaRequest
{
    [StringLength(200, ErrorMessage = "Título deve ter no máximo 200 caracteres")]
    [MinLength(1, ErrorMessage = "Título deve ter pelo menos 1 caractere")]
    public string? Titulo { get; set; }  // ← Nullable: só atualiza se fornecido

    [StringLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
    public string? Descricao { get; set; }  // ← Já estava correto

    [DataType(DataType.DateTime)]
    public DateTime? DataConclusao { get; set; }  // ← Nullable: só atualiza se fornecido

    public PrioridadeTarefa? Prioridade { get; set; }  // ← Nullable: só atualiza se fornecido

    public bool? Concluida { get; set; }  // ← Nullable: só atualiza se fornecido

    [DataType(DataType.DateTime)]
    public DateTime? DataLembrete { get; set; }  // ← Já estava correto
}
