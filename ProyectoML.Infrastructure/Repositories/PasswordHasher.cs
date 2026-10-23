

using Microsoft.AspNetCore.Identity;
using ProyectoML.Application.Auth;
using ProyectoML.Domain.Entities;
namespace ProyectoML.Infrastructure.Repositories;

public class PasswordHasher : IPasswordHasher
{
    private readonly IPasswordHasher<Usuario> _passwordHasher;

    public PasswordHasher()
    {
        _passwordHasher = new PasswordHasher<Usuario>();
    }

    public string Hash(string password)
    {
        return _passwordHasher.HashPassword(null!, password);
    }
    public bool Verificar(string hashedPassword, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(null!, hashedPassword, password);
        return result != PasswordVerificationResult.Failed;
    }
}
