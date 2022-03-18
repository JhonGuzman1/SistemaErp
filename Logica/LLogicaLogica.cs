using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Logica
{
    public abstract class LLogicaLogica<T,E> where T : DbContext
    {
        protected abstract T GetEsquema();
    }
}
