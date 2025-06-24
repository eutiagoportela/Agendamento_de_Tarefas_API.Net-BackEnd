using AutoMapper;
using Microsoft.Extensions.Logging;
using Tarefa.Comunicacao.Requests.Tarefa;
using Tarefa.Comunicacao.Responses.Tarefa;
using Tarefa.Domain.Repositorios.Interfaces;
using Tarefa.Exceptions;

namespace Tarefa.Aplicacao.UseCases.Tarefa.ListarTarefasPaginadas;

public class ListarTarefasPaginadasUseCase : IListarTarefasPaginadasUseCase
{
    private readonly ITarefaRepository _tarefaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ListarTarefasPaginadasUseCase> _logger;

    public ListarTarefasPaginadasUseCase(
        ITarefaRepository tarefaRepository,
        IUsuarioRepository usuarioRepository,
        IMapper mapper,
        ILogger<ListarTarefasPaginadasUseCase> logger)
    {
        _tarefaRepository = tarefaRepository;
        _usuarioRepository = usuarioRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PaginatedResponse<TarefaResponse>> ExecuteAsync(int usuarioId, ListarTarefasRequest request)
    {
        // Verificar se usuário existe
        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);
        if (usuario == null)
            throw new UsuarioNaoEncontradoException();

        // Validar parâmetros de paginação
        var pageNumber = Math.Max(1, request.PageNumber);
        var pageSize = Math.Min(Math.Max(1, request.PageSize), 50); // Máximo 50 por página

        _logger.LogInformation("Listagem paginada de tarefas - Usuário: {UserId}, Página: {PageNumber}, Filtros: {SearchTerm}",
            usuarioId, pageNumber, request.SearchTerm);

        // Calcular offset
        var skip = (pageNumber - 1) * pageSize;

        // Obter tarefas com paginação
        var (tarefas, totalCount) = await _tarefaRepository.ObterTarefasComPaginacaoAsync(
            usuarioId,
            skip,
            pageSize,
            request.SearchTerm?.Trim(),
            request.Concluida,
            request.Prioridade);

        // Mapear para response
        var tarefasResponse = _mapper.Map<List<TarefaResponse>>(tarefas);

        // Calcular informações de paginação
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var paginatedResponse = new PaginatedResponse<TarefaResponse>
        {
            Data = tarefasResponse,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasNextPage = pageNumber < totalPages,
            HasPreviousPage = pageNumber > 1
        };

        _logger.LogInformation("Listagem concluída - Usuário: {UserId}, Total: {TotalCount}",
            usuarioId, totalCount);

        return paginatedResponse;
    }
}
