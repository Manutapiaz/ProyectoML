using ProyectoML.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoML.Application.MercadoLibre
{
    public interface ICredencialMercadoLibreRepository
    {
   
        Task AddAsync(CredencialMercadoLibre credencialMercadoLibre);

        Task SaveChangesAsync();

        Task<bool> ExistsByIdAsync(long id);
        Task<CredencialMercadoLibre?> GetByUserIdAsync(long userId);


    }
}
