using Microsoft.Extensions.Options;
using Microsoft.Identity.Client.NativeInterop;
using ProyectoML.Application.MercadoLibre;
using ProyectoML.Application.MercadoLibre.PublicacionML;
using ProyectoML.Application.Exceptions;
using ProyectoML.Domain.Entities;
using ProyectoML.Domain.Enums;
using ProyectoML.Infrastructure.MercadoLibre;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ProyectoML.Infrastructure.Client;

public class MercadoLibreApiClient : IMercadoLibreApiClient
{
    private readonly HttpClient _httpClient;



    public MercadoLibreApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DataUserDto> ObtenerCuentaAsync(string token)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/users/me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var ml = await response.Content.ReadFromJsonAsync<MlUserResponse>(JsonOptions);
        return new DataUserDto(ml!.Id, ml.Nickname, ml.SiteId, ml.RegistrationDate);

    }

    public async Task<List<SugerenciaCategoria>> SugerirCategoriaAsync(string nombreProducto)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"/sites/MLC/domain_discovery/search?q={Uri.EscapeDataString(nombreProducto)}");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var ml = await response.Content.ReadFromJsonAsync<List<MlCategoriaSugerida>>(JsonOptions);

        return ml!
            .Select(c => new SugerenciaCategoria(c.CategoryId, c.CategoryName, c.DomainName))
            .ToList();
    }

    public async Task<List<AtributoCategoria>> ObtenerAtributosCategoriaAsync(string categoryId, string token)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"/categories/{categoryId}/attributes");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var ml = await response.Content.ReadFromJsonAsync<List<MlAttributeCategory>>(JsonOptions);

        return ml!
            .Where(a => !a.Tags.ReadOnly)
            .Where(a => !a.Tags.Hidden || a.Tags.Required || a.Tags.ConditionalRequired)
            .Select(a => new AtributoCategoria(
                a.Id,
                a.Name,
                a.ValueType,
                a.Tags.Required,
                a.Tags.ConditionalRequired,
                a.Values?.Select(v => new ValorAtributo(v.Id, v.Name)).ToList() ?? []))
            .ToList();
    }

    public async Task<ItemPublicado> PublicarItemAsync(ItemMercadoLibre item, string token)
    {
        var body = new MlItemRequest(
            item.FamilyName,
            item.CategoryId,
            item.Price,
            "CLP",
            item.AvailableQuantity,
            "buy_it_now",
            item.ListingTypeId,
            "new",
            item.Pictures.Select(url => new MlPicture(url)).ToList(),
            item.Atributos.Select(a => new MlItemAttribute(a.Id, a.ValueId, a.ValueName)).ToList());

        using var request = new HttpRequestMessage(HttpMethod.Post, "/items");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(body, options: JsonOptions);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
            throw await CrearExcepcionAsync(response);

        var ml = await response.Content.ReadFromJsonAsync<MlItemResponse>(JsonOptions);
        return new ItemPublicado(ml!.Id, MapearEstado(ml.Status), ml.Permalink);
    }

    public async Task<ItemPublicado> BuscarPublicacionAsync(string idProductoMLC, string token)
    {

        using var request = new HttpRequestMessage(HttpMethod.Get, $"/items/{idProductoMLC}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
            throw await CrearExcepcionAsync(response);
        var ml = await response.Content.ReadFromJsonAsync<MlItemResponse>(JsonOptions);
        return new ItemPublicado(ml!.Id, MapearEstado(ml.Status), ml.Permalink);
    }
    private static EstadoPublicacion MapearEstado(string status) => status switch
    {
        "active" => EstadoPublicacion.Activo,
        "closed" => EstadoPublicacion.Cerrada,
        "paused" => EstadoPublicacion.Pausada,
        _ => EstadoPublicacion.EnRevision
    };

    private static async Task<MercadoLibreException> CrearExcepcionAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        var detalle = body;

        try
        {
            var error = JsonSerializer.Deserialize<MlError>(body, JsonOptions);
            var causas = error?.Cause?
                .Where(c => c.Type != "warning" && !string.IsNullOrWhiteSpace(c.Message))
                .Select(c => c.Message)
                .ToList() ?? [];

            if (causas.Count > 0) detalle = string.Join(" | ", causas);
            else if (!string.IsNullOrWhiteSpace(error?.Message)) detalle = error.Message;
        }
        catch (JsonException)
        {
        }

        return new MercadoLibreException(detalle, (int)response.StatusCode);
    }

    private record MlItemRequest(
        string FamilyName,
        string CategoryId,
        int Price,
        string CurrencyId,
        int AvailableQuantity,
        string BuyingMode,
        string ListingTypeId,
        string Condition,
        List<MlPicture> Pictures,
        List<MlItemAttribute> Attributes);

    private record MlPicture(string Source);

    private record MlItemAttribute(string Id, string? ValueId, string? ValueName);

    private record MlItemResponse(string Id, string Status, string? Permalink);

    private record MlError(string? Message, List<MlErrorCause>? Cause);

    private record MlErrorCause(string? Type, string? Message);



    private record MlCategoriaSugerida(string DomainName, string CategoryId, string CategoryName);

    private record MlAttributeCategory(
        string Id,
        string Name,
        string ValueType,
        MlAttributeTags Tags,
        List<MlAttributeValue>? Values);

    private record MlAttributeTags(
        bool Required,
        bool ConditionalRequired,
        bool Hidden,
        bool ReadOnly);

    private record MlAttributeValue(string Id, string Name);

    private record MlUserResponse(long Id, string Nickname, string SiteId, DateTimeOffset RegistrationDate);
    private static readonly JsonSerializerOptions JsonOptions =
       new()
       {
           PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
           DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
       };
}
