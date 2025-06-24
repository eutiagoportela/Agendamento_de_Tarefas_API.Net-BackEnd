using AutoMapper;

using Tarefa.Comunicacao.Responses.Tarefa;
using Tarefa.Domain.Repositorios.Interfaces;
using Tarefa.Exceptions;


namespace Tarefa.Aplicacao.UseCases.Tarefa.MarcarTarefaComoLembreteEnviado;

public class MarcarTarefaComoLembreteEnviado : IMarcarTarefaComoLembreteEnviado
{
    private readonly ITarefaRepository _tarefaRepository;
    private readonly IMapper _mapper;

    public MarcarTarefaComoLembreteEnviado(
        ITarefaRepository tarefaRepository,
        IMapper mapper)
    {
        _tarefaRepository = tarefaRepository;
        _mapper = mapper;
    }

    public async Task<TarefaResponse> ExecuteAsync(int tarefaId, int usuarioId)
    {
        try
        {
            // Verificar se a tarefa existe e pertence ao usuário
            var tarefa = await _tarefaRepository.ObterPorIdAsync(tarefaId);
            if (tarefa == null)
                throw new TarefaNaoEncontradaException();
            
            if (tarefa.UsuarioId != usuarioId)
                throw new AcessoNegadoException("Usuário não tem permissão para marcar lembrete desta tarefa");
        
            // Marcar lembrete como enviado
            await _tarefaRepository.MarcarLembreteEnviadoAsync(tarefaId);
        
            // Retornar tarefa atualizada
            tarefa.LembreteEnviado = true; // Atualizar propriedade localmente
            return _mapper.Map<TarefaResponse>(tarefa);
        }
        catch (TarefaNaoEncontradaException)
        {
            throw;
        }
        catch (AcessoNegadoException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao marcar lembrete como enviado: {ex.Message}", ex);
        }
    }
}
