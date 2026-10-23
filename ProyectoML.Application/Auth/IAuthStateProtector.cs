using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoML.Application.Auth;

public interface IAuthStateProtector
{
    string Proteger(int usuarioId);
    int? Desproteger(string state);
}
