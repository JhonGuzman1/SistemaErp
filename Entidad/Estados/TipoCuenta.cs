using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidad.Estados
{
    public enum TipoCuenta: int
    {

        Global = 1,
        Detalle = 2

    }

    public enum TipoEstadoResultado: int{
        Ingreso = 1,
        Egreso = 2
    }

}
