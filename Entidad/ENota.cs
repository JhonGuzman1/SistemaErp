using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidad
{
    public class ENota
    {
        public long IdNota { get; set; }
        public long NroNota { get; set; }
        public string Fecha { get; set; }
        public string Descripcion { get; set; }
        public double Total { get; set; }
        public string Estado { get; set; }


    }

    public class ERDetalleNotaCompra
    {
        public long IdArticulo { get; set; }
        public long NroLote { get; set; }
        public string NombreArticulo { get; set; }
        public int Cantidad { get; set; }
        public double Precio { get; set; }
        public double SubTotal { get; set; }
        public string FechaVencimiento { get; set; }
    }
    public class EDetalleNotaCompra
    {
        public long IdArticulo { get; set; }
        public string NombreArticulo { get; set; }
        public int Cantidad { get; set; }
        public double Precio { get; set; }
        public double SubTotal { get; set; }
        public DateTime? FechaVencimiento { get; set; }
    }

    public class EDetalleNotaCompraTotal
    {
        public double Total { get; set; }
    }

    public class EDetalleNotaCompraVer
    {
        public int Ver { get; set; }
    }

    public class ERDetalleNotaVenta
    {
        public long IdArticulo { get; set; }
        public long NroLote { get; set; }
        public string NombreArticulo { get; set; }
        public int Cantidad { get; set; }
        public double Precio { get; set; }
        public double SubTotal { get; set; }
    }

    public class EDetalleNotaVenta
    {
        public long IdArticulo { get; set; }
        public long NroLote { get; set; }
        public string NombreArticulo { get; set; }
        public int Cantidad { get; set; }
        public double Precio { get; set; }
        public double SubTotal { get; set; }
    }

    public class EDetalleNotaVentaTotal
    {
        public double Total { get; set; }
    }

    public class EDetalleNotaVentaVer
    {
        public int Ver { get; set; }
    }
}
