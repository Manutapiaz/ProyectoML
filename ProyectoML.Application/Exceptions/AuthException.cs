using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoML.Application.Exceptions;


public class AuthException(string message) : Exception(message);    