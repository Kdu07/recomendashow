using System;
using System.Collections.Generic;
using BackEnd.Aventura.Repository;

namespace BackEnd.Aventura.Services
{
    public interface ITeatroService
    {
        IEnumerable<Teatro> GetAllTeatros();
        Teatro? GetTeatroById(Guid id);
        IReadOnlyCollection<Espetaculo> GetEspetaculos(Guid teatroId);
    }
}
