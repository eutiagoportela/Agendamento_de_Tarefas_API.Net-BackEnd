using Microsoft.EntityFrameworkCore;
using Tarefa.Domain.Entidades;
using Tarefa.Domain.Enum;
using Tarefa.Domain.Repositorios.Interfaces;

namespace Tarefa.Infraestrutura.Repositorios.Implementacoes;

public class TarefaRepository : ITarefaRepository
{
    private readonly PostgreSqlDbContext _context;

    public TarefaRepository(PostgreSqlDbContext context)
    {
        _context = context;
    }

    // ===== OPERAÇÕES BÁSICAS (ESSENCIAIS) =====

    public async Task<Tarefas?> ObterPorIdAsync(int id)
    {
        return await _context.Tarefas
            .Include(t => t.Usuario)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<Tarefas>> ObterPorUsuarioAsync(int usuarioId)
    {
        return await _context.Tarefas
            .Where(t => t.UsuarioId == usuarioId)
            .OrderByDescending(t => t.DataCriacao)
            .ToListAsync();
    }

    public async Task<Tarefas> CriarAsync(Tarefas tarefa)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.Tarefas.Add(tarefa);
            var x = await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return tarefa;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Tarefas> AtualizarAsync(Tarefas tarefa)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.Tarefas.Update(tarefa);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return tarefa;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task DeletarAsync(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var tarefa = await _context.Tarefas.FindAsync(id);
            if (tarefa != null)
            {
                _context.Tarefas.Remove(tarefa);
                await _context.SaveChangesAsync();
            }
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // ===== FILTROS ESPECÍFICOS 

    public async Task<List<Tarefas>> ObterTarefasPorPrioridadeAsync(int usuarioId, PrioridadeTarefa prioridade)
    {
        return await _context.Tarefas
            .Where(t => t.UsuarioId == usuarioId && t.Prioridade == prioridade)
            .OrderByDescending(t => t.DataCriacao)
            .ToListAsync();
    }

    public async Task<List<Tarefas>> ObterTarefasPendentesAsync(int usuarioId)
    {
        return await _context.Tarefas
            .Where(t => t.UsuarioId == usuarioId && !t.Concluida)
            .OrderBy(t => t.DataConclusao)
            .ThenByDescending(t => t.Prioridade)
            .ToListAsync();
    }

    // ===== SISTEMA DE LEMBRETES 

    public async Task<List<Tarefas>> ObterTarefasComLembreteAsync()
    {
        return await _context.Tarefas
            .Include(t => t.Usuario) // Para ter dados do usuário no lembrete
            .Where(t => t.DataLembrete.HasValue &&
                       t.DataLembrete <= DateTime.UtcNow &&
                       !t.LembreteEnviado &&
                       !t.Concluida)
            .OrderBy(t => t.DataLembrete)
            .ToListAsync();
    }

    public async Task MarcarLembreteEnviadoAsync(int tarefaId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var tarefa = await _context.Tarefas.FindAsync(tarefaId);
            if (tarefa != null)
            {
                tarefa.LembreteEnviado = true;
                await _context.SaveChangesAsync();
            }
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<Tarefas>> ObterTarefasConcluidasAsync(int usuarioId)
    {
        return await _context.Tarefas
            .Where(t => t.UsuarioId == usuarioId && t.Concluida)
            .OrderBy(t => t.DataConclusao)
            .ToListAsync();
    }

    public async Task<(List<Tarefas> tarefas, int totalCount)> ObterTarefasComPaginacaoAsync(
    int usuarioId,
    int skip,
    int take,
    string? searchTerm = null,
    bool? concluida = null,
    PrioridadeTarefa? prioridade = null)
    {
        var query = _context.Tarefas
            .Where(t => t.UsuarioId == usuarioId)
            .AsQueryable();

        // Filtro por texto (título ou descrição)
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var termo = searchTerm.ToLowerInvariant();
            query = query.Where(t =>
                t.Titulo.ToLower().Contains(termo) ||
                (t.Descricao != null && t.Descricao.ToLower().Contains(termo)));
        }

        // Filtro por status (concluída/pendente)
        if (concluida.HasValue)
        {
            query = query.Where(t => t.Concluida == concluida.Value);
        }

        // Filtro por prioridade
        if (prioridade.HasValue)
        {
            query = query.Where(t => t.Prioridade == prioridade.Value);
        }

        var totalCount = await query.CountAsync();

        var tarefas = await query
            .OrderByDescending(t => t.DataCriacao)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return (tarefas, totalCount);
    }
}