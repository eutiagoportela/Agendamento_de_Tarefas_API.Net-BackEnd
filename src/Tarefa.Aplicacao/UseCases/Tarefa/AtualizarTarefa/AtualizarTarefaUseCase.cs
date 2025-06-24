using AutoMapper;
using Tarefa.Comunicacao.Requests;
using Tarefa.Comunicacao.Responses.Tarefa;
using Tarefa.Domain.Repositorios.Interfaces;
using Tarefa.Exceptions;

namespace Tarefa.Aplicacao.UseCases.Tarefa.AtualizarTarefa;

public class AtualizarTarefaUseCase : IAtualizarTarefaUseCase
{
    private readonly ITarefaRepository _tarefaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper _mapper;

    public AtualizarTarefaUseCase(
        ITarefaRepository tarefaRepository,
        IUsuarioRepository usuarioRepository,
        IMapper mapper)
    {
        _tarefaRepository = tarefaRepository;
        _usuarioRepository = usuarioRepository;
        _mapper = mapper;
    }

    public async Task<TarefaResponse> ExecuteAsync(int tarefaId, AtualizarTarefaRequest request, int usuarioId)
    {
        // Verificar se o usuário existe
        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);
        if (usuario == null)
            throw new UsuarioNaoEncontradoException();

        // Verificar se a tarefa existe e pertence ao usuário
        var tarefa = await _tarefaRepository.ObterPorIdAsync(tarefaId);
        if (tarefa == null)
            throw new TarefaNaoEncontradaException();

        if (tarefa.UsuarioId != usuarioId)
            throw new AcessoNegadoException("Usuário não tem permissão para atualizar esta tarefa");

        // Atualizar apenas campos fornecidos
        if (!string.IsNullOrWhiteSpace(request.Titulo))
            tarefa.Titulo = request.Titulo.Trim();

        if (request.Descricao != null)
            tarefa.Descricao = request.Descricao.Trim();

        if (request.DataConclusao.HasValue)
            tarefa.DataConclusao = request.DataConclusao.Value;

        if (request.Prioridade.HasValue)
            tarefa.Prioridade = request.Prioridade.Value;

        if (request.DataLembrete.HasValue)
        {
            tarefa.DataLembrete = request.DataLembrete.Value;
            tarefa.LembreteEnviado = false; // Resetar flag quando definir novo lembrete
        }

        if (request.Concluida.HasValue)
        {
            tarefa.Concluida = request.Concluida.Value;
        }

        // Sempre atualizar DataAtualizacao
        tarefa.DataAtualizacao = DateTime.UtcNow; 

        var tarefaAtualizada = await _tarefaRepository.AtualizarAsync(tarefa);

        return _mapper.Map<TarefaResponse>(tarefaAtualizada);
    }
}