using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidad
{
    public class ERDatosBasico
    {
        public string Usuario { get; set; }
        public string FechaActual { get; set; }

    }

    public class ERDatosBasicoGestion
    {
        public string Usuario { get; set; }
        public string NombreEmpresa {get;set;}
        public string FechaActual { get; set; }

    }

    public class ERDatosBasicoPeriodo
    {
        public string Usuario { get; set; }
        public string NombreEmpresa { get; set; }
        public string NombreGestion { get; set; }
        public string FechaActual { get; set; }

    }

    public class ERDatosBasicoCuenta
    {
        public string Usuario { get; set; }
        public string NombreEmpresa { get; set; }
        public string FechaActual { get; set; }

    }

    public class ERDatosBasicoComprobante
    {
        public string Usuario { get; set; }
        public string NombreEmpresa { get; set; }
        public string NombreGestion { get; set; }
        public string NombrePeriodo { get; set; }
        public string FechaActual { get; set; }
        public string Moneda { get; set; }
    }

    public class ETotalEstado
    {
        public double Total { get; set; }
    }

}
