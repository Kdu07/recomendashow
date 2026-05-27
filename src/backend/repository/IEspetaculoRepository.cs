using System;
using System.Collections.Generic;

namespace BackEnd.Aventura.Repository
{
    public interface IEspetaculoRepository
    {
        IEnumerable<Espetaculo> GetAll();
        Espetaculo? GetById(Guid id);
        IEnumerable<Espetaculo> GetByGenero(string genero);
    }
}
