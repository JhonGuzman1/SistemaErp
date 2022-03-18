using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidad
{

    public class EComprobante
    {
        public long Serie { get; set; }
        public string Estado { get; set; }
        public string TipoComprobante { get; set; }
        public string Moneda { get; set; }
        public string Fecha { get; set; }
        public string Glosa { get; set; }
        public double TipoCambio { get; set; }
    }
    public class EDetalleComprobante
    {
        public long IdCuenta { get; set; }
        public string Cuenta { get; set; }
        public string Glosa { get; set; }
        public double Debe { get; set; }
        public double Haber { get; set; }

    }

    public class EDetalleTotal
    {
        public double TotalDebe { get; set; }
        public double TotalHaber { get; set; }
    }


    public class DetalleEstado
    {
        public int Estado { get; set; }
    }

    public class ELibroDiario
    {
        public string CodigoCuenta { get; set; }
        public string Cuenta { get; set; }
        public string Fecha { get; set; }
        public double Debe { get; set; }
        public double Haber { get; set; }

    }

    public class ESumasSaldos
    {
        public string CodigoCuenta { get; set; }
        public string Cuenta { get; set; }
        public string Fecha { get; set; }
        public double SumasDebe { get; set; }
        public double SumasHaber { get; set; }
        public double SaldosDebe { get; set; }
        public double SaldoHaber { get; set; }

    }

    public class EDetalleTotalSumasSaldos
    {
        public double TotalSumasDebe { get; set; }
        public double TotalSumasHaber { get; set; }
        public double TotalSaldosDebe { get; set; }
        public double TotalSaldoHaber { get; set; }
    }

    public class ELibroMayoCabezera
    {
        public long IdCuenta { get; set; }
        public string Cuenta { get; set; }
      
    }
    public class ELibroMayor
    {
     
        public long IdCuenta { get; set; }
        public string Cuenta { get; set; }
        public string Fecha { get; set; }
        public long NroComprobante { get; set; }
        public string Tipo { get; set; }
        public string Glosa { get; set; }
        public double Debe { get; set; }
        public double Haber { get; set; }
        public double Saldo { get; set; }
    }


    public class EEstadoResultado
    {

        public long IdCuenta { get; set; }
        public string Cuenta { get; set; }
        public string TipoComprobante { get; set; }
      
        public double Total { get; set; }
      
       
    }

}
