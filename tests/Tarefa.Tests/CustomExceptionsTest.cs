using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tarefa.Exceptions;
using Xunit;

namespace Tarefa.Tests;

public class CustomExceptionsTest
{
    [Fact]
    public void EmailJaExisteException_DeveTerMensagemPadrao()
    {
        // Act
        var exception = new EmailJaExisteException();

        // Assert
        exception.Should().BeOfType<EmailJaExisteException>();
        exception.Message.Should().Contain("já está em uso");
    }

    [Fact]
    public void UsuarioNaoEncontradoException_DeveTerMensagemPadrao()
    {
        // Act
        var exception = new UsuarioNaoEncontradoException();

        // Assert
        exception.Should().BeOfType<UsuarioNaoEncontradoException>();
        exception.Message.Should().Contain("não foi encontrado");
    }

    [Fact]
    public void TarefaNaoEncontradaException_DeveTerMensagemPadrao()
    {
        // Act
        var exception = new TarefaNaoEncontradaException();

        // Assert
        exception.Should().BeOfType<TarefaNaoEncontradaException>();
        exception.Message.Should().Contain("não foi encontrada");
    }

    [Fact]
    public void AcessoNegadoException_DeveTerMensagemPadrao()
    {
        // Act
        var exception = new AcessoNegadoException();

        // Assert
        exception.Should().BeOfType<AcessoNegadoException>();
        exception.Message.Should().Contain("acesso negado");
    }
}
