using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tarefa.Aplicacao.UseCases.Usuario.CriarUsuario;
using Tarefa.Aplicacao.UseCases.Usuario.ObterUsuario;
using Tarefa.Aplicacao.UseCases.Usuario.AtualizarUsuario;
using Tarefa.Aplicacao.UseCases.Usuario.DeletarUsuario;
using Tarefa.Comunicacao.Requests.Usuario;
using Tarefa.Comunicacao.Responses;
using Tarefa.Comunicacao.Responses.Usuario;
using Tarefa.Exceptions;
using System.Security.Claims;

namespace Tarefa.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly ICriarUsuarioUseCase _criarUsuarioUseCase;
    private readonly IObterUsuarioUseCase _obterUsuarioUseCase;
    private readonly IAtualizarUsuarioUseCase _atualizarUsuarioUseCase;
    private readonly IDeletarUsuarioUseCase _deletarUsuarioUseCase;
    private readonly ILogger<UsuarioController> _logger;

    public UsuarioController(
        ICriarUsuarioUseCase criarUsuarioUseCase,
        IObterUsuarioUseCase obterUsuarioUseCase,
        IAtualizarUsuarioUseCase atualizarUsuarioUseCase,
        IDeletarUsuarioUseCase deletarUsuarioUseCase,
        ILogger<UsuarioController> logger)
    {
        _criarUsuarioUseCase = criarUsuarioUseCase;
        _obterUsuarioUseCase = obterUsuarioUseCase;
        _atualizarUsuarioUseCase = atualizarUsuarioUseCase;
        _deletarUsuarioUseCase = deletarUsuarioUseCase;
        _logger = logger;
    }

    [HttpPost("registrar")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RespostaPadrao<UsuarioResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(RespostaPadrao<UsuarioResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RespostaPadrao<UsuarioResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Registrar([FromBody] CriarUsuarioRequest request)
    {
        _logger.LogInformation("Iniciando criação de usuário com email: {Email}", request.Email);

        try
        {
            var usuario = await _criarUsuarioUseCase.ExecuteAsync(request);

            _logger.LogInformation("Usuário {UserId} criado com sucesso com email: {Email}", usuario.Id, request.Email);
            var resposta = RespostaPadrao<UsuarioResponse>.ComSucesso(usuario, "Usuário criado com sucesso");
            return Created($"/api/usuario/{usuario.Id}", resposta);
        }
        catch (EmailJaExisteException ex)
        {
            _logger.LogWarning("Tentativa de criar usuário com email já existente: {Email}", request.Email);
            var resposta = RespostaPadrao<UsuarioResponse>.ComErro(ex.Message);
            return BadRequest(resposta);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Dados inválidos enviados para criar usuário: {ErrorMessage}", ex.Message);
            var resposta = RespostaPadrao<UsuarioResponse>.ComErro(ex.Message);
            return BadRequest(resposta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno inesperado ao criar usuário com email: {Email}", request.Email);
            var resposta = RespostaPadrao<UsuarioResponse>.ComErro("Erro interno do servidor");
            return StatusCode(500, resposta);
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(RespostaPadrao<UsuarioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaPadrao<UsuarioResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(RespostaPadrao<UsuarioResponse>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(RespostaPadrao<UsuarioResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterPorId([FromRoute] int id)
    {
        _logger.LogInformation("Consultando usuário por ID: {UserId}", id);

        try
        {
            // Verificar se usuário está tentando acessar seus próprios dados
            var usuarioLogadoId = GetUsuarioLogadoId();
            if (usuarioLogadoId != id && !User.IsInRole("Admin"))
            {
                _logger.LogWarning("Usuário {UsuarioLogadoId} tentou acessar dados do usuário {UserId} sem permissão",
                    usuarioLogadoId, id);
                var respostaForbidden = RespostaPadrao<UsuarioResponse>.ComErro("Acesso negado");
                return Forbid();
            }

            var usuario = await _obterUsuarioUseCase.ExecuteAsync(id);

            _logger.LogInformation("Usuário {UserId} encontrado com sucesso", id);
            var resposta = RespostaPadrao<UsuarioResponse>.ComSucesso(usuario);
            return Ok(resposta);
        }
        catch (UsuarioNaoEncontradoException ex)
        {
            _logger.LogWarning("Usuário não encontrado para ID: {UserId}", id);
            var resposta = RespostaPadrao<UsuarioResponse>.ComErro(ex.Message);
            return NotFound(resposta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno inesperado ao obter usuário por ID: {UserId}", id);
            var resposta = RespostaPadrao<UsuarioResponse>.ComErro("Erro interno do servidor");
            return StatusCode(500, resposta);
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(RespostaPadrao<UsuarioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaPadrao<UsuarioResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RespostaPadrao<UsuarioResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(RespostaPadrao<UsuarioResponse>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(RespostaPadrao<UsuarioResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Atualizar([FromRoute] int id, [FromBody] AtualizarUsuarioRequest request)
    {
        _logger.LogInformation("Iniciando atualização do usuário ID: {UserId}", id);

        try
        {
            // Verificar se usuário está tentando atualizar seus próprios dados
            var usuarioLogadoId = GetUsuarioLogadoId();
            if (usuarioLogadoId != id)
            {
                _logger.LogWarning("Usuário {UsuarioLogadoId} tentou atualizar dados do usuário {UserId} sem permissão",
                    usuarioLogadoId, id);
                var respostaForbidden = RespostaPadrao<UsuarioResponse>.ComErro("Acesso negado");
                return BadRequest(respostaForbidden);
            }

            var usuario = await _atualizarUsuarioUseCase.ExecuteAsync(id, request);

            _logger.LogInformation("Usuário {UserId} atualizado com sucesso", id);
            var resposta = RespostaPadrao<UsuarioResponse>.ComSucesso(usuario, "Usuário atualizado com sucesso");
            return Ok(resposta);
        }
        catch (UsuarioNaoEncontradoException ex)
        {
            _logger.LogWarning("Usuário não encontrado para atualização. ID: {UserId}", id);
            var resposta = RespostaPadrao<UsuarioResponse>.ComErro(ex.Message);
            return NotFound(resposta);
        }
        catch (EmailJaExisteException ex)
        {
            _logger.LogWarning("Tentativa de atualizar usuário {UserId} com email já existente", id);
            var resposta = RespostaPadrao<UsuarioResponse>.ComErro(ex.Message);
            return BadRequest(resposta);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Dados inválidos enviados para atualizar usuário {UserId}: {ErrorMessage}", id, ex.Message);
            var resposta = RespostaPadrao<UsuarioResponse>.ComErro(ex.Message);
            return BadRequest(resposta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno inesperado ao atualizar usuário ID: {UserId}", id);
            var resposta = RespostaPadrao<UsuarioResponse>.ComErro("Erro interno do servidor");
            return StatusCode(500, resposta);
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(RespostaPadrao<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaPadrao<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(RespostaPadrao<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(RespostaPadrao<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Deletar([FromRoute] int id)
    {
        _logger.LogInformation("Iniciando exclusão do usuário ID: {UserId}", id);

        try
        {
            // Verificar se usuário está tentando deletar seus próprios dados ou é admin
            var usuarioLogadoId = GetUsuarioLogadoId();
            if (usuarioLogadoId != id)
            {
                _logger.LogWarning("Usuário {UsuarioLogadoId} tentou deletar usuário {UserId} sem permissão",
                    usuarioLogadoId, id);
                var respostaForbidden = RespostaPadrao<object>.ComErro("Acesso negado");
                return BadRequest(respostaForbidden);
            }

            await _deletarUsuarioUseCase.ExecuteAsync(id);

            _logger.LogInformation("Usuário {UserId} deletado com sucesso", id);
            var resposta = RespostaPadrao<object>.ComSucesso(null, "Usuário deletado com sucesso");
            return Ok(resposta);
        }
        catch (UsuarioNaoEncontradoException ex)
        {
            _logger.LogWarning("Usuário não encontrado para exclusão. ID: {UserId}", id);
            var resposta = RespostaPadrao<object>.ComErro(ex.Message);
            return NotFound(resposta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno inesperado ao deletar usuário ID: {UserId}", id);
            var resposta = RespostaPadrao<object>.ComErro("Erro interno do servidor");
            return StatusCode(500, resposta);
        }
    }

    private int GetUsuarioLogadoId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            throw new UnauthorizedAccessException("Token inválido ou usuário não identificado");
        }
        return userId;
    }
}