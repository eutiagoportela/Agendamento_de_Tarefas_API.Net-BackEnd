using Microsoft.AspNetCore.Mvc;
using Tarefa.Aplicacao.UseCases.Login.DoLogin;
using Tarefa.Comunicacao.Requests.Usuario;
using Tarefa.Comunicacao.Responses;
using Tarefa.Comunicacao.Responses.Usuario;
using Tarefa.Exceptions;

namespace Tarefa.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IDoLoginUseCase _doLoginUseCase;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IDoLoginUseCase doLoginUseCase, ILogger<AuthController> logger)
    {
        _doLoginUseCase = doLoginUseCase;
        _logger = logger;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(RespostaPadrao<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaPadrao<LoginResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RespostaPadrao<LoginResponse>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(RespostaPadrao<LoginResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Tentativa de login para usuário: {UsuarioNome}", request.Email);

        try
        {
            var response = await _doLoginUseCase.ExecuteAsync(request);

            if (response != null)
            {
                _logger.LogInformation("Login realizado com sucesso para usuário: {UsuarioEmail}", request.Email);
                var resposta = RespostaPadrao<LoginResponse>.ComSucesso(response, "Login realizado com sucesso");
                return Ok(resposta);
            }

            _logger.LogWarning("Tentativa de login com credenciais inválidas para usuário: {UsuarioEmail}", request.Email);
            var respostaUnauthorized = RespostaPadrao<LoginResponse>.ComErro("Credenciais inválidas" );
            return Unauthorized(respostaUnauthorized);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Dados inválidos no login para usuário {UsuarioEmail}: {ErrorMessage}",
                             request.Email, ex.Message);
            var resposta = RespostaPadrao<LoginResponse>.ComErro(ex.Message);
            return BadRequest(resposta);
        }
        catch (UsuarioNaoEncontradoException ex)
        {
            _logger.LogWarning("Usuário não encontrado no login: {UsuarioEmail}", request.Email);
            var resposta = RespostaPadrao<LoginResponse>.ComErro("Credenciais inválidas: "+ ex.Message);
            return Unauthorized(resposta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao realizar login para usuário: {UsuarioEmail}", request.Email);
            var resposta = RespostaPadrao<LoginResponse>.ComErro("Erro interno do servidor: " + ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, resposta);
        }
    }
}

