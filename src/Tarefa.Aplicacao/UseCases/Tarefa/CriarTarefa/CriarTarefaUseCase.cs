using AutoMapper;
using Tarefa.Comunicacao.Requests.Tarefa;
using Tarefa.Comunicacao.Responses.Tarefa;
using Tarefa.Domain.Repositorios.Interfaces;
using Tarefa.Exceptions;

namespace Tarefa.Aplicacao.UseCases.Tarefa.CriarTarefa;

public class CriarTarefaUseCase : ICriarTarefaUseCase
{
    private readonly ITarefaRepository _tarefaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper _mapper;

    public CriarTarefaUseCase(
        ITarefaRepository tarefaRepository,
        IUsuarioRepository usuarioRepository,
        IMapper mapper)
    {
        _tarefaRepository = tarefaRepository;
        _usuarioRepository = usuarioRepository;
        _mapper = mapper;
    }

    public async Task<TarefaResponse> ExecuteAsync(CriarTarefaRequest request, int usuarioId)
    {
        // Verificar se o usuário existe
        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);
        if (usuario == null)
            throw new UsuarioNaoEncontradoException();

        // ✅ Criar usando método factory + validação da entidade
        var novaTarefa = new Tarefas
        {
            Titulo = request.Titulo.Trim(),
            Descricao = request.Descricao?.Trim() ?? string.Empty,
            DataConclusao = request.DataConclusao,
            Prioridade = request.Prioridade,
            UsuarioId = usuarioId,
            Concluida = false,
            DataCriacao = DateTime.UtcNow,
            DataAtualizacao = DateTime.UtcNow
        };

        // Configurar lembrete se fornecido usando método da entidade
        if (request.DataLembrete.HasValue)
        {
            novaTarefa.DefinirLembrete(request.DataLembrete.Value);
        }

        // Validar usando método da entidade
        novaTarefa.ValidarCriacao();



        // Salvar no repositório
        var tarefaCriada = await _tarefaRepository.CriarAsync(novaTarefa);

        // Retornar resposta mapeada
        return _mapper.Map<TarefaResponse>(tarefaCriada);
    }
}