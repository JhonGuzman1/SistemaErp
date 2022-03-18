using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidad.Estados
{
    public enum EstadoLote: int
    {
        Activo = 1,
        Anulado = 2,
        SinStock = 3
    }
}
