using ProyectoML.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoML.Application.MercadoLibre
{
    public interface ICredencialMercadoLibreRepository
    {
        Task<CredencialMercadoLibre> GetByIdAsync(int id);
        Task AddAsync(CredencialMercadoLibre credencialMercadoLibre);
        Task<bool> ExistsByIdAsync(int id);

    }
}
