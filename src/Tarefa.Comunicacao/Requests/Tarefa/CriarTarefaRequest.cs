
using System.ComponentModel.DataAnnotations;
using Tarefa.Domain.Enum;

namespace Tarefa.Comunicacao.Requests.Tarefa;

public class CriarTarefaRequest
{
    [Required(ErrorMessage = "Título é obrigatório")]
    [StringLength(200, ErrorMessage = "Título deve ter no máximo 200 caracteres")]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "Data de conclusão é obrigatória")]
    [DataType(DataType.DateTime)]
    public DateTime DataConclusao { get; set; }

    [Required(ErrorMessage = "Prioridade é obrigatória")]
    public PrioridadeTarefa Prioridade { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? DataLembrete { get; set; }
}
