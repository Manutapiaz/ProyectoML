namespace ProyectoML.Application.MercadoLibre;

public record TokenResult(string accessToken, int expiresIn, string refreshToken, long userId);


