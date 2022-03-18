using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidad
{
    public class ECuenta
    {

        public long idCuenta { get; set; }
        public long id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string text { get; set; }
        public int TipoCuenta { get; set; }
        public int Nivel { get; set; }
        public long IdUsuario { get; set; }
        public long IdEmpresa { get; set; }
        public Nullable<long> IdCuentaPadre { get; set; }
        public List<ECuenta> children { get; set; }

    }
}
