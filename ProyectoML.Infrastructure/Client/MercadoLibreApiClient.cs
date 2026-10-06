using Microsoft.Extensions.Options;
using ProyectoML.Application.MercadoLibre;
using ProyectoML.Infrastructure.MercadoLibre;
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
        //var json = await response.Content.ReadAsStringAsync();
        var ml = await response.Content.ReadFromJsonAsync<MlUserResponse>(JsonOptions);
        return new DataUserDto(ml!.Id, ml.Nickname, ml.SiteId, ml.RegistrationDate);

    }
    private record MlUserResponse(long Id, string Nickname, string SiteId, DateTimeOffset RegistrationDate);
    private static readonly JsonSerializerOptions JsonOptions =
       new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
}
