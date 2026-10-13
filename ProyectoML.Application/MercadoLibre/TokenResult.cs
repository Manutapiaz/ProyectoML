namespace ProyectoML.Application.MercadoLibre;

public record TokenResult(string AccessToken, long ExpiresIn, string RefreshToken, long UserId);
public record SugerenciaCategoria(string CategoryId, string CategoryName, string DomainName);
public record AtributoCategoria(
    string Id,
    string Name,
    string ValueType,
    bool Required,
    bool ConditionalRequired,
    List<ValorAtributo> Values);

public record ValorAtributo(string Id, string Name);



