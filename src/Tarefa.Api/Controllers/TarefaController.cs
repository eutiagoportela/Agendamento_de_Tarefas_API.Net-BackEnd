using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tarefa.Aplicacao.UseCases.Tarefa.ObterTarefa;
using Tarefa.Aplicacao.UseCases.Tarefa.ObterTarefasUsuario;
using Tarefa.Aplicacao.UseCases.Tarefa.ObterTarefasPendentes;
using Tarefa.Aplicacao.UseCases.Tarefa.ObterTarefasPorPrioridade;
using Tarefa.Aplicacao.UseCases.Tarefa.AtualizarTarefa;
using Tarefa.Aplicacao.UseCases.Tarefa.MarcarTarefaComoConcluida;
using Tarefa.Aplicacao.UseCases.Tarefa.DeletarTarefa;
using Tarefa.Aplicacao.UseCases.Tarefa.CriarTarefa;
using Tarefa.Comunicacao.Requests;
using Tarefa.Comunicacao.Responses;
using Tarefa.Domain.Enum;
using Tarefa.Exceptions;
using Tarefa.Comunicacao.Requests.Tarefa;
using Tarefa.Comunicacao.Responses.Tarefa;
using Tarefa.Aplicacao.UseCases.Tarefa.ObterTarefasConcluidas;
using Tarefa.Aplicacao.UseCases.Tarefa.MarcarTarefaComoLembreteEnviado;



namespace Tarefa.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TarefaController : ControllerBase
{
    private readonly ICriarTarefaUseCase _criarTarefaUseCase;
    private readonly IObterTarefaUseCase _obterTarefaUseCase;
    private readonly IObterTarefasUsuarioUseCase _obterTarefasUsuarioUseCase;
    private readonly IObterTarefasPendentesUseCase _obterTarefasPendentesUseCase;
    private readonly IObterTarefasPorPrioridadeUseCase _obterTarefasPorPrioridadeUseCase;
    private readonly IAtualizarTarefaUseCase _atualizarTarefaUseCase;
    private readonly IMarcarTarefaComoConcluidaUseCase _marcarTarefaComoConcluidaUseCase;
    private readonly IDeletarTarefaUseCase _deletarTarefaUseCase;
    private readonly IObterTarefasConcluidasUseCase _obterTarefasConcluidasUseCase;
    private readonly IMarcarTarefaComoLembreteEnviado _marcarTarefaComoLembreteEnviado;

    private readonly ILogger<TarefaController> _logger;

    public TarefaController(
        ICriarTarefaUseCase criarTarefaUseCase,
        IObterTarefaUseCase obterTarefaUseCase,
        IObterTarefasUsuarioUseCase obterTarefasUsuarioUseCase,
        IObterTarefasPendentesUseCase obterTarefasPendentesUseCase,
        IObterTarefasPorPrioridadeUseCase obterTarefasPorPrioridadeUseCase,
        IAtualizarTarefaUseCase atualizarTarefaUseCase,
        IMarcarTarefaComoConcluidaUseCase marcarTarefaComoConcluidaUseCase,
        IDeletarTarefaUseCase deletarTarefaUseCase,
        IObterTarefasConcluidasUseCase obterTarefasConcluidasUseCase,
        IMarcarTarefaComoLembreteEnviado marcarTarefaComoLembreteEnviado,
        ILogger<TarefaController> logger)
    {
        _criarTarefaUseCase = criarTarefaUseCase;
        _obterTarefaUseCase = obterTarefaUseCase;
        _obterTarefasUsuarioUseCase = obterTarefasUsuarioUseCase;
        _obterTarefasPendentesUseCase = obterTarefasPendentesUseCase;
        _obterTarefasPorPrioridadeUseCase = obterTarefasPorPrioridadeUseCase;
        _atualizarTarefaUseCase = atualizarTarefaUseCase;
        _marcarTarefaComoConcluidaUseCase = marcarTarefaComoConcluidaUseCase;
        _deletarTarefaUseCase = deletarTarefaUseCase;
        _obterTarefasConcluidasUseCase = obterTarefasConcluidasUseCase;
        _marcarTarefaComoLembreteEnviado = marcarTarefaComoLembreteEnviado;
        _logger = logger;
    }

    private int ObterUsuarioId()
    {
        var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(usuarioIdClaim ?? "0");
    }

    [HttpPost]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Criar([FromBody] CriarTarefaRequest request)
    {
        var usuarioId = ObterUsuarioId();
        _logger.LogInformation("Iniciando criação de tarefa para usuário {UserId}", usuarioId);

        try
        {
            var tarefa = await _criarTarefaUseCase.ExecuteAsync(request, usuarioId);

            _logger.LogInformation("Tarefa {TarefaId} criada com sucesso para usuário {UserId}", tarefa.Id, usuarioId);
            var resposta = RespostaPadrao<TarefaResponse>.ComSucesso(tarefa, "Tarefa criada com sucesso");
            return Created($"/api/tarefa/{tarefa.Id}", resposta);
        }
        catch (UsuarioNaoEncontradoException ex)
        {
            _logger.LogWarning("Usuário {UserId} não encontrado ao criar tarefa", usuarioId);
            var resposta = RespostaPadrao<TarefaResponse>.ComErro("Erro ao criar tarefa: " + ex.Message);
            return BadRequest(resposta);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Dados inválidos enviados pelo usuário {UserId}: {ErrorMessage}", usuarioId, ex.Message);
            var resposta = RespostaPadrao<TarefaResponse>.ComErro(ex.Message);
            return BadRequest(resposta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno inesperado ao criar tarefa para usuário {UserId}", usuarioId);
            var resposta = RespostaPadrao<TarefaResponse>.ComErro("Erro interno do servidor: " + ex.Message);
            return StatusCode(500, resposta);
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RespostaPadrao<TarefaResponse>>> ObterPorId([FromRoute] int id)
    {
        var usuarioId = ObterUsuarioId();
        _logger.LogInformation("Usuário {UserId} consultando tarefa {TarefaId}", usuarioId, id);

        try
        {
            var tarefa = await _obterTarefaUseCase.ExecuteAsync(id, usuarioId);

            _logger.LogInformation("Tarefa {TarefaId} encontrada para usuário {UserId}", id, usuarioId);
            var resposta = RespostaPadrao<TarefaResponse>.ComSucesso(tarefa);
            return Ok(resposta);
        }
        catch (TarefaNaoEncontradaException ex)
        {
            _logger.LogWarning("Tarefa {TarefaId} não encontrada", id);
            var resposta = RespostaPadrao<TarefaResponse>.ComErro(ex.Message);
            return NotFound(resposta);
        }
        catch (AcessoNegadoException ex)
        {
            _logger.LogWarning("Usuário {UserId} tentou acessar tarefa não autorizada {TarefaId}", usuarioId, id);
            var resposta = RespostaPadrao<TarefaResponse>.ComErro(ex.Message);
            return NotFound(resposta);
        }
        catch (UsuarioNaoEncontradoException ex)
        {
            _logger.LogWarning("Usuário {UserId} não encontrado", usuarioId);
            var resposta = RespostaPadrao<TarefaResponse>.ComErro(ex.Message);
            return BadRequest(resposta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno inesperado ao obter tarefa {TarefaId} para usuário {UserId}", id, usuarioId);
            var resposta = RespostaPadrao<TarefaResponse>.ComErro("Erro interno do servidor: " + ex.Message);
            return StatusCode(500, resposta);
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(RespostaPadrao<List<TarefaResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaPadrao<List<TarefaResponse>>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RespostaPadrao<List<TarefaResponse>>>> ListarTarefas()
    {
        var usuarioId = ObterUsuarioId();
        _logger.LogInformation("Usuário {UserId} listando suas tarefas", usuarioId);

        try
        {
            var tarefas = await _obterTarefasUsuarioUseCase.ExecuteAsync(usuarioId);

            _logger.LogInformation("Usuário {UserId} possui {TotalTarefas} tarefas", usuarioId, tarefas.Count);
            var resposta = RespostaPadrao<List<TarefaResponse>>.ComSucesso(tarefas);
            return Ok(resposta);
        }
        catch (UsuarioNaoEncontradoException ex)
        {
            _logger.LogWarning("Usuário {UserId} não encontrado", usuarioId);
            var resposta = RespostaPadrao<List<TarefaResponse>>.ComErro(ex.Message);
            return BadRequest(resposta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno inesperado ao listar tarefas do usuário {UserId}", usuarioId);
            var resposta = RespostaPadrao<List<TarefaResponse>>.ComErro("Erro interno do servidor: " + ex.Message);
            return StatusCode(500, resposta);
        }
    }

    [HttpGet("pendentes")]
    [ProducesResponseType(typeof(RespostaPadrao<List<TarefaResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaPadrao<List<TarefaResponse>>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RespostaPadrao<List<TarefaResponse>>>> ListarTarefasPendentes()
    {
        var usuarioId = ObterUsuarioId();
        _logger.LogInformation("Usuário {UserId} consultando tarefas pendentes", usuarioId);

        try
        {
            var tarefas = await _obterTarefasPendentesUseCase.ExecuteAsync(usuarioId);

            _logger.LogInformation("Usuário {UserId} possui {TotalPendentes} tarefas pendentes", usuarioId, tarefas.Count);
            var resposta = RespostaPadrao<List<TarefaResponse>>.ComSucesso(tarefas);
            return Ok(resposta);
        }
        catch (UsuarioNaoEncontradoException ex)
        {
            _logger.LogWarning("Usuário {UserId} não encontrado", usuarioId);
            var resposta = RespostaPadrao<List<TarefaResponse>>.ComErro(ex.Message);
            return BadRequest(resposta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno inesperado ao obter tarefas pendentes do usuário {UserId}", usuarioId);
            var resposta = RespostaPadrao<List<TarefaResponse>>.ComErro("Erro interno do servidor: " + ex.Message);
            return StatusCode(500, resposta);
        }
    }

    [HttpGet("concluidas")]
    [ProducesResponseType(typeof(RespostaPadrao<List<TarefaResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaPadrao<List<TarefaResponse>>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RespostaPadrao<List<TarefaResponse>>>> ListarTarefasConcluidas()
    {
        var usuarioId = ObterUsuarioId();
        _logger.LogInformation("Usuário {UserId} consultando tarefas concluídas", usuarioId);

        try
        {
            var tarefas = await _obterTarefasConcluidasUseCase.ExecuteAsync(usuarioId);

            _logger.LogInformation("Usuário {UserId} possui {TotalConcluidas} tarefas concluídas", usuarioId, tarefas.Count);
            var resposta = RespostaPadrao<List<TarefaResponse>>.ComSucesso(tarefas);
            return Ok(resposta);
        }
        catch (UsuarioNaoEncontradoException ex)
        {
            _logger.LogWarning("Usuário {UserId} não encontrado", usuarioId);
            var resposta = RespostaPadrao<List<TarefaResponse>>.ComErro(ex.Message);
            return BadRequest(resposta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno inesperado ao obter tarefas concluídas do usuário {UserId}", usuarioId);
            var resposta = RespostaPadrao<List<TarefaResponse>>.ComErro("Erro interno do servidor: " + ex.Message);
            return StatusCode(500, resposta);
        }
    }


    [HttpGet("prioridade/{prioridade}")]
    [ProducesResponseType(typeof(RespostaPadrao<List<TarefaResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaPadrao<List<TarefaResponse>>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RespostaPadrao<List<TarefaResponse>>>> ListarPorPrioridade([FromRoute] PrioridadeTarefa prioridade)
    {
        var usuarioId = ObterUsuarioId();
        _logger.LogInformation("Usuário {UserId} filtrando tarefas por prioridade {Prioridade}", usuarioId, prioridade);

        try
        {
            var tarefas = await _obterTarefasPorPrioridadeUseCase.ExecuteAsync(usuarioId, prioridade);

            _logger.LogInformation("Usuário {UserId} possui {TotalTarefas} tarefas com prioridade {Prioridade}",
                                 usuarioId, tarefas.Count, prioridade);
            var resposta = RespostaPadrao<List<TarefaResponse>>.ComSucesso(tarefas);
            return Ok(resposta);
        }
        catch (UsuarioNaoEncontradoException ex)
        {
            _logger.LogWarning("Usuário {UserId} não encontrado", usuarioId);
            var resposta = RespostaPadrao<List<TarefaResponse>>.ComErro(ex.Message);
            return BadRequest(resposta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno inesperado ao obter tarefas por prioridade {Prioridade} do usuário {UserId}",
                           prioridade, usuarioId);
            var resposta = RespostaPadrao<List<TarefaResponse>>.ComErro("Erro interno do servidor: " + ex.Message);
            return StatusCode(500, resposta);
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RespostaPadrao<TarefaResponse>>> Atualizar([FromRoute] int id, [FromBody] AtualizarTarefaRequest request)
    {
        var usuarioId = ObterUsuarioId();
        _logger.LogInformation("Usuário {UserId} atualizando tarefa {TarefaId}", usuarioId, id);

        try
        {
            var tarefa = await _atualizarTarefaUseCase.ExecuteAsync(id, request, usuarioId);

            _logger.LogInformation("Tarefa {TarefaId} atualizada com sucesso pelo usuário {UserId}", id, usuarioId);
            var resposta = RespostaPadrao<TarefaResponse>.ComSucesso(tarefa, "Tarefa atualizada com sucesso");
            return Ok(resposta);
        }
        catch (TarefaNaoEncontradaException ex)
        {
            _logger.LogWarning("Tarefa {TarefaId} não encontrada", id);
            var resposta = RespostaPadrao<TarefaResponse>.ComErro(ex.Message);
            return NotFound(resposta);
        }
        catch (AcessoNegadoException ex)
        {
            _logger.LogWarning("Usuário {UserId} tentou atualizar tarefa não autorizada {TarefaId}", usuarioId, id);
            var resposta = RespostaPadrao<TarefaResponse>.ComErro(ex.Message);
            return NotFound(resposta);
        }
        catch (UsuarioNaoEncontradoException ex)
        {
            _logger.LogWarning("Usuário {UserId} não encontrado", usuarioId);
            var resposta = RespostaPadrao<TarefaResponse>.ComErro(ex.Message);
            return BadRequest(resposta);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Dados inválidos na atualização da tarefa {TarefaId} pelo usuário {UserId}: {ErrorMessage}",
                             id, usuarioId, ex.Message);
            var resposta = RespostaPadrao<TarefaResponse>.ComErro(ex.Message);
            return BadRequest(resposta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno inesperado ao atualizar tarefa {TarefaId} para usuário {UserId}", id, usuarioId);
            var resposta = RespostaPadrao<TarefaResponse>.ComErro("Erro interno do servidor: " + ex.Message);
            return StatusCode(500, resposta);
        }
    }

    [HttpPatch("{id}/concluir")]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RespostaPadrao<TarefaResponse>>> MarcarComoConcluida([FromRoute] int id)
    {
        var usuarioId = ObterUsuarioId();
        _logger.LogInformation("Usuário {UserId} marcando tarefa {TarefaId} como concluída", usuarioId, id);

        try
        {
            var tarefa = await _marcarTarefaComoConcluidaUseCase.ExecuteAsync(id, usuarioId);

            _logger.LogInformation("Tarefa {TarefaId} marcada como concluída pelo usuário {UserId}", id, usuarioId);
            var resposta = RespostaPadrao<TarefaResponse>.ComSucesso(tarefa, "Tarefa marcada como concluída");
            return Ok(resposta);
        }
        catch (TarefaNaoEncontradaException ex)
        {
            _logger.LogWarning("Tarefa {TarefaId} não encontrada", id);
            var resposta = RespostaPadrao<TarefaResponse>.ComErro(ex.Message);
            return NotFound(resposta);
        }
        catch (AcessoNegadoException ex)
        {
            _logger.LogWarning("Usuário {UserId} tentou concluir tarefa não autorizada {TarefaId}", usuarioId, id);
            var resposta = RespostaPadrao<TarefaResponse>.ComErro(ex.Message);
            return NotFound(resposta);
        }
        catch (UsuarioNaoEncontradoException ex)
        {
            _logger.LogWarning("Usuário {UserId} não encontrado", usuarioId);
            var resposta = RespostaPadrao<TarefaResponse>.ComErro(ex.Message);
            return BadRequest(resposta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno inesperado ao marcar tarefa {TarefaId} como concluída para usuário {UserId}",
                           id, usuarioId);
            var resposta = RespostaPadrao<TarefaResponse>.ComErro("Erro interno do servidor: " + ex.Message);
            return StatusCode(500, resposta);
        }
    }

    [HttpPatch("{id}/lembrete-enviado")]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(RespostaPadrao<TarefaResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RespostaPadrao<TarefaResponse>>> MarcarLembreteComoEnviado([FromRoute] int id)
    {
        var usuarioId = ObterUsuarioId();
        _logger.LogInformation("Usuário {UserId} marcando lembrete da tarefa {TarefaId} como enviado", usuarioId, id);
        try
        {
            var tarefa = await _marcarTarefaComoLembreteEnviado.ExecuteAsync(id, usuarioId);
            _logger.LogInformation("Lembrete da tarefa {TarefaId} marcado como enviado pelo usuário {UserId}", id, usuarioId);
            var resposta = RespostaPadrao<TarefaResponse>.ComSucesso(tarefa, "Lembrete marcado como enviado");
            return Ok(resposta);
        }
        catch (TarefaNaoEncontradaException ex)
        {
            _logger.LogWarning("Tarefa {TarefaId} não encontrada", id);
            var resposta = RespostaPadrao<TarefaResponse>.ComErro(ex.Message);
            return NotFound(resposta);
        }
        catch (AcessoNegadoException ex)
        {
            _logger.LogWarning("Usuário {UserId} tentou marcar lembrete de tarefa não autorizada {TarefaId}", usuarioId, id);
            var resposta = RespostaPadrao<TarefaResponse>.ComErro(ex.Message);
            return NotFound(resposta);
        }
        catch (UsuarioNaoEncontradoException ex)
        {
            _logger.LogWarning("Usuário {UserId} não encontrado", usuarioId);
            var resposta = RespostaPadrao<TarefaResponse>.ComErro(ex.Message);
            return BadRequest(resposta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno inesperado ao marcar lembrete da tarefa {TarefaId} como enviado para usuário {UserId}",
                           id, usuarioId);
            var resposta = RespostaPadrao<TarefaResponse>.ComErro("Erro interno do servidor: " + ex.Message);
            return StatusCode(500, resposta);
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(RespostaPadrao<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaPadrao<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(RespostaPadrao<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RespostaPadrao<object>>> Deletar([FromRoute] int id)
    {
        var usuarioId = ObterUsuarioId();
        _logger.LogWarning("Usuário {UserId} iniciando exclusão da tarefa {TarefaId}", usuarioId, id);

        try
        {
            await _deletarTarefaUseCase.ExecuteAsync(id, usuarioId);

            _logger.LogWarning("Tarefa {TarefaId} excluída permanentemente pelo usuário {UserId}", id, usuarioId);
            var resposta = RespostaPadrao<object>.ComSucesso(null, "Tarefa deletada com sucesso");
            return Ok(resposta);
        }
        catch (TarefaNaoEncontradaException ex)
        {
            _logger.LogWarning("Tarefa {TarefaId} não encontrada", id);
            var resposta = RespostaPadrao<object>.ComErro(ex.Message);
            return NotFound(resposta);
        }
        catch (AcessoNegadoException ex)
        {
            _logger.LogWarning("Usuário {UserId} tentou excluir tarefa não autorizada {TarefaId}", usuarioId, id);
            var resposta = RespostaPadrao<object>.ComErro(ex.Message);
            return NotFound(resposta);
        }
        catch (UsuarioNaoEncontradoException ex)
        {
            _logger.LogWarning("Usuário {UserId} não encontrado", usuarioId);
            var resposta = RespostaPadrao<object>.ComErro(ex.Message);
            return BadRequest(resposta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno inesperado ao excluir tarefa {TarefaId} para usuário {UserId}", id, usuarioId);
            var resposta = RespostaPadrao<object>.ComErro("Erro interno do servidor: " + ex.Message);
            return StatusCode(500, resposta);
        }
    }
}