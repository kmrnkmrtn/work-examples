using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PotyogosAmoba.Model.Persistance
{
    public interface IPotyogosAmobaDataAccess
    {
        Task<PotyogosAmobaTable> LoadAsync(String path);
        Task SaveAsync(String path, PotyogosAmobaTable table);
    }
}
