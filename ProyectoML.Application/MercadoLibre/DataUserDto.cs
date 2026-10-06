using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoML.Application.MercadoLibre;

public record DataUserDto(long Id, string Nickname, string SiteId, DateTimeOffset RegistrationDate);

