using ProyectoML.Application.Exceptions;
using ProyectoML.Domain.Entities;
using ProyectoML.Domain.Exceptions;

namespace ProyectoML.Application.Auth;

public class AuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    public AuthService(IUsuarioRepository usuarioRepository, IPasswordHasher passwordHasher, ITokenService tokenService)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }
    public async Task<string> LoginAsync(LoginRequest login)
    {
        var email = Usuario.NormalizarEmail(login.Email);     
        var usuario = await _usuarioRepository.GetByEmailAsync(email);
        if (usuario == null || !_passwordHasher.Verificar(usuario.PasswordHash, login.Password))
        {
            throw new AuthException("Credenciales inválidas");
        }
        return _tokenService.GenerarToken(usuario);
    }
    public async Task RegisterAsync(RegisterRequest request)
    {
        var email = Usuario.NormalizarEmail(request.Email);
        if (string.IsNullOrEmpty(request.Password) || request.Password.Length < 8)
        {
            throw new DomainException("La contraseña debe tener al menos 8 caracteres");
        }
        if (await _usuarioRepository.ExistByEmailAsync(email))
        {
            throw new ConflictException("El correo electrónico ya está registrado");
        }
        var usuario = new Usuario(request.NombreUsuario, email, _passwordHasher.Hash(request.Password));
        await _usuarioRepository.AddAsync(usuario);

    }


}
