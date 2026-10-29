using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;


namespace ProyectoML.Application.MercadoLibre.NotificacionML;

public record NotificationRequest(
   string Resource,
   [property: JsonPropertyName("user_id")] long UserId,
   string Topic,
    [property: JsonPropertyName("application_id")] long ApplicationId,
   int Attempts,
   string Sent,
   string Received);


public record NotificationResponse(
   string Resource,
   [property: JsonPropertyName("user_id")] long UserId,
   string Topic,
    [property: JsonPropertyName("application_id")] long ApplicationId,
   int Attempts,
   string Sent,
   string Received);



public record Orden(long Id, string Estado, List<ItemOrden> Items);
public record ItemOrden(string ItemId, int Cantidad);

