
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Tarefa.Infraestrutura.Security;
using Xunit;

namespace Tarefa.Tests;

public class PasswordHasherTest
{
    private readonly PasswordHasher _passwordHasher;

    public PasswordHasherTest()
    {
        _passwordHasher = new PasswordHasher();
    }

    [Fact]
    public void HashPassword_DeveGerarHashDiferente_DaSenhaOriginal()
    {
        // Arrange
        var senhaOriginal = "minhasenha123";

        // Act
        var hash = _passwordHasher.HashPassword(senhaOriginal);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        hash.Should().NotBe(senhaOriginal);
        hash.Should().StartWith("$2a$"); // BCrypt hash pattern
    }

    [Fact]
    public void VerifyPassword_DeveRetornarTrue_QuandoSenhaCorreta()
    {
        // Arrange
        var senhaOriginal = "senhasecreta456";
        var hash = _passwordHasher.HashPassword(senhaOriginal);

        // Act
        var isValid = _passwordHasher.VerifyPassword(senhaOriginal, hash);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_DeveRetornarFalse_QuandoSenhaIncorreta()
    {
        // Arrange
        var senhaCorreta = "senhaCorreta123";
        var senhaIncorreta = "senhaErrada456";
        var hash = _passwordHasher.HashPassword(senhaCorreta);

        // Act
        var isValid = _passwordHasher.VerifyPassword(senhaIncorreta, hash);

        // Assert
        isValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("123456")]
    [InlineData("senhamuitorande123456789")]
    [InlineData("Senha@Com#Caracteres$Especiais")]
    public void HashPassword_DeveFuncionar_ComDiferentesTiposDeSenha(string senha)
    {
        // Act
        var hash = _passwordHasher.HashPassword(senha);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        hash.Should().NotBe(senha);

        // Verificar se o hash pode ser verificado
        var isValid = _passwordHasher.VerifyPassword(senha, hash);
        isValid.Should().BeTrue();
    }
}
