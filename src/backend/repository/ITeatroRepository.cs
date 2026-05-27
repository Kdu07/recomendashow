using System;
using System.Collections.Generic;

namespace BackEnd.Aventura.Repository
{
    public interface ITeatroRepository
    {
        IEnumerable<Teatro> GetAll();
        Teatro? GetById(Guid id);
    }
}
