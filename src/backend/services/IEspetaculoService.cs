using System;
using System.Collections.Generic;
using BackEnd.Aventura.Repository;

namespace BackEnd.Aventura.Services
{
    public interface IEspetaculoService
    {
        IEnumerable<Espetaculo> GetAllEspetaculos();
        Espetaculo? GetEspetaculoById(Guid id);
        IEnumerable<Espetaculo> GetEspetaculosByGenero(string genero);
    }
}
