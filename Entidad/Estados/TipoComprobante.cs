using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidad.Estados
{
    public enum TipoComprobante: int
    {

        Ingreso = 1,
        Egreso = 2,
        Traspaso = 3,
        Apertura =  4,
        Ajuste = 5

    }
}
