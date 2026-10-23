using ProyectoML.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoML.Application.Auth
{
    public interface IUsuarioRepository
    {
        Task<bool> ExistByEmailAsync(string email);

        Task<Usuario?> GetByEmailAsync(string email);

        Task AddAsync(Usuario usuario);
    }
}
