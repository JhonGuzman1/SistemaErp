using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidad;
using Datos;
using Entidad.Estados;
using System.Transactions;

namespace Logica
{
    public class LNotaCompra:LLogica<Nota>
    {


        public List<Nota> listarNotaCompra(long idempresa)
        {
            using (var esquema = GetEsquema())
            {

                try
                {

                 
                    var notas = (from x in esquema.Nota
                                        where x.IdEmpresa == idempresa
                                        && x.TipoNota == (int)TipoNota.NotaCompra
                                        orderby x.Fecha descending
                                        select x).ToList();



                    return notas;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la lista de notas de compra");
                }

            }
        }

        public long obtenerNroNotaCompra(long idempresa)
        {
            using (var esquema = GetEsquema())
            {

                try
                {

                    long NroNota = 1;




                    var nota = (from x in esquema.Nota
                                        where x.IdEmpresa == idempresa
                                        && x.TipoNota == (int)TipoNota.NotaCompra
                                        orderby x.IdNota descending
                                        select x).FirstOrDefault();

                    if (nota != null)
                    {
                        NroNota = NroNota + nota.NroNota;
                    }


                    return NroNota;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la nota de compra");
                }

            }
        }

        public Nota ObtenerNotaGeneralXComprobante(long idempresa, long idcomprobante)
        {
            using (var esquema = GetEsquema())
            {

                try
                {

                   


                    var nota = (from x in esquema.Nota
                                where x.IdEmpresa == idempresa
                                && x.IdComprobante == idcomprobante
                                select x).FirstOrDefault();

                    return nota;
                  

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la nota ");
                }

            }
        }

        public Nota RegistroNotaCompra(Nota Entidad, List<EDetalleNotaCompra> detalleNotaCompra, EDetalleNotaCompraTotal total)
        {
            using (var esquema = GetEsquema())
            {
                using (var transaction = new TransactionScope())
                {

                    try
                    {



                        if(detalleNotaCompra.Count == 0)
                        {
                            throw new MensageException("Ingrese el detalle de la nota de compra");
                        }

                        var empresa = LEmpresa.Logica.LEmpresa.obtenerEmpresa(Entidad.IdEmpresa);


                        if(empresa.Integracion == (int)Integracion.Si)
                        {

                            List<EDetalleComprobante> detalle = new List<EDetalleComprobante>();
                            EDetalleTotal totalcomprobante = new EDetalleTotal();


                            var cuentacaja = LCuenta.Logica.LCuenta.obtenerCuenta(empresa.IdCuentaCaja.Value);
                            var cuentacreditofiscal = LCuenta.Logica.LCuenta.obtenerCuenta(empresa.IdCuentaCreditoFiscal.Value);
                            var cuentacompra = LCuenta.Logica.LCuenta.obtenerCuenta(empresa.IdCuentaCompras.Value);

                            double CuentaCompraTotal = 0;
                            double CuentaCreditoFiscalTotal = 0;
                            double CuentaCajaTotal = 0;

                            CuentaCajaTotal = total.Total;
                            CuentaCreditoFiscalTotal = Math.Round((CuentaCajaTotal * 0.13), 2);
                            CuentaCompraTotal = Math.Round((CuentaCajaTotal - CuentaCreditoFiscalTotal), 2);



                            EDetalleComprobante CuentaCompra = new EDetalleComprobante();
                            CuentaCompra.IdCuenta = cuentacompra.idCuenta;
                            CuentaCompra.Cuenta = cuentacompra.Codigo + " " + cuentacompra.Nombre;
                            CuentaCompra.Glosa = "Compras de Mercaderias";
                            CuentaCompra.Debe = CuentaCompraTotal;
                            CuentaCompra.Haber = 0;
                            detalle.Add(CuentaCompra);


                            EDetalleComprobante CuentaCreditoFiscal = new EDetalleComprobante();
                            CuentaCreditoFiscal.IdCuenta = cuentacreditofiscal.idCuenta;
                            CuentaCreditoFiscal.Cuenta = cuentacreditofiscal.Codigo + " " + cuentacreditofiscal.Nombre;
                            CuentaCreditoFiscal.Glosa = "Compras de Mercaderias";
                            CuentaCreditoFiscal.Debe = CuentaCreditoFiscalTotal;
                            CuentaCreditoFiscal.Haber = 0;
                            detalle.Add(CuentaCreditoFiscal);

                            EDetalleComprobante CuentaCaja = new EDetalleComprobante();
                            CuentaCaja.IdCuenta = cuentacaja.idCuenta;
                            CuentaCaja.Cuenta = cuentacaja.Codigo + " " + cuentacaja.Nombre;
                            CuentaCaja.Glosa = "Compras de Mercaderias";
                            CuentaCaja.Debe = 0;
                            CuentaCaja.Haber = CuentaCajaTotal;
                            detalle.Add(CuentaCaja);


                            totalcomprobante.TotalHaber = CuentaCaja.Haber;
                            totalcomprobante.TotalDebe = Math.Round((CuentaCreditoFiscal.Debe + CuentaCompra.Debe), 2);


                            var moneda = LMoneda.Logica.LMoneda.obtenerMonedadXempresa(empresa.idEmpresa);
                            Comprobante ComprobanteEntidad = new Comprobante()
                            {
                                Serie = LComprobante.Logica.LComprobante.obtenerSerie(empresa.idEmpresa),
                                Glosa = "Compras de Mercaderias",
                                Fecha = Entidad.Fecha,
                                TipoCambio = moneda.Cambio.Value,
                                Estado = (int)EstadoComprobante.Abierto,
                                TipoComprobante = (int)TipoComprobante.Egreso,
                                IdEmpresa = empresa.idEmpresa,
                                IdMoneda = moneda.IdMonedaPrincipal,
                                IdUsuario = Entidad.IdUsuario

                            };

                            ComprobanteEntidad = LComprobante.Logica.LComprobante.Registro(ComprobanteEntidad, detalle, totalcomprobante);
                           
                            Entidad.IdComprobante = ComprobanteEntidad.IdComprobante;

                        }




                        Entidad.NroNota = obtenerNroNotaCompra(Entidad.IdEmpresa);
                        Entidad.Total = total.Total;
                        esquema.Nota.Add(Entidad);
                        esquema.SaveChanges();

                        foreach (var i in detalleNotaCompra)
                        {
                            long nrolote = 1;
                            var ultimolote = (from x in esquema.Lote
                                            where x.IdArticulo == i.IdArticulo
                                            orderby x.NroLote descending
                                            select x).FirstOrDefault();
                          

                            Lote lote = new Lote();
                            lote.IdArticulo = i.IdArticulo;
                            if (ultimolote != null)
                            {
                                lote.NroLote = ultimolote.NroLote + nrolote;
                            }
                            else
                            {
                                lote.NroLote = nrolote;
                            }
                            lote.IdNota = Entidad.IdNota;
                            lote.PrecioCompra = i.Precio;
                            lote.Cantidad = i.Cantidad;
                            lote.Stock = i.Cantidad;
                            lote.Estado = Entidad.Estado;
                            lote.FechaIngreso = Entidad.Fecha;
                            lote.FechaVencimiento = i.FechaVencimiento;
                            esquema.Lote.Add(lote);
                            esquema.SaveChanges();

                            var articulo = (from x in esquema.Articulo
                                            where x.IdArticulo == lote.IdArticulo
                                            select x).FirstOrDefault();

                            articulo.Cantidad = articulo.Cantidad + lote.Cantidad;
                            esquema.SaveChanges();
    
                        }

                        transaction.Complete();
                        return Entidad;
                      

                    }
                    catch(MensageException ex)
                    {
                        throw new MensageException(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        throw new MensageException("Error no se puedo obtener la lista de comprobantes");
                    }
                }
            }
        }




        public Nota ObtenerNotaCompra(long idnota, long idempresa)
        {
            using (var esquema = GetEsquema())
            {

                try
                {

                    var nota = (from x in esquema.Nota
                                where x.IdEmpresa == idempresa
                                && x.TipoNota == (int)TipoNota.NotaCompra
                                && x.IdNota == idnota
                                select x).FirstOrDefault();

                    if (nota != null)
                    {
                        return nota;

                    }
                    else
                    {
                        throw new MensageException("Error no se puedo obtener la nota de compra");
                    }



                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la nota de compra");
                }

            }
        }

        public List<EDetalleNotaCompra> ListarDetalleNotaCompra(long idnota)
        {
            using (var esquema = GetEsquema())
            {

                try
                {

                    var lotes = (from x in esquema.Lote
                                 where x.IdNota == idnota
                                 select x).ToList();

                    List<EDetalleNotaCompra> detalleNotaCompra = new List<EDetalleNotaCompra>();


                    foreach(var i in lotes)
                    {
                        EDetalleNotaCompra d = new EDetalleNotaCompra();
                        d.IdArticulo = i.IdArticulo;
                        d.NombreArticulo = i.Articulo.Nombre;
                        d.Cantidad = i.Cantidad;
                        d.Precio = i.PrecioCompra;
                        d.FechaVencimiento = i.FechaVencimiento;
                        d.SubTotal = Math.Round((d.Cantidad * d.Precio), 2);
                        detalleNotaCompra.Add(d);
                    }

                    return detalleNotaCompra;
                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo listar el detalle de la nota de compra");
                }

            }

        }


        public Nota AnularNotaCompra(long idnota, long idempresa)
        {
            using (var esquema = GetEsquema())
            {

                try
                {

                    var nota = (from x in esquema.Nota
                                where x.IdEmpresa == idempresa
                                && x.TipoNota == (int)TipoNota.NotaCompra
                                && x.IdNota == idnota
                                select x).FirstOrDefault();

                    if (nota != null)
                    {

                        if (nota.Comprobante != null)
                        {
                            nota.Comprobante.Estado = (int)EstadoComprobante.Anualdo;
                        }

                        foreach (var i in nota.Lote)
                        {


                            if (i.Cantidad != i.Stock)
                            {
                                throw new MensageException("No se puedo anular la nota de compra por que tiene un venta");
                            }

                        }

                        nota.Estado = (int)EstadoNota.Anulado;
                        foreach (var i in nota.Lote)
                        {


                            var articulo = (from x in esquema.Articulo
                                            where x.IdArticulo == i.IdArticulo
                                            select x).FirstOrDefault();

                          


                            i.Estado = (int)EstadoNota.Anulado;
                            articulo.Cantidad = articulo.Cantidad - i.Cantidad;
                            esquema.SaveChanges();
                        }
                     
                        esquema.SaveChanges();

                        return nota;

                    }
                    else
                    {
                        throw new MensageException("Error no se puedo obtener la nota de compra");
                    }



                }
                catch (MensageException ex)
                {
                    throw new MensageException(ex.Message);
                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la nota de compra");
                }

            }
        }


        public List<ERDatosBasicoCuenta> ReporteDatosBasicoNotaCompra(string usuario, string empresa)
        {
            try
            {


                List<ERDatosBasicoCuenta> basicos = new List<ERDatosBasicoCuenta>();
                ERDatosBasicoCuenta eRDatosBasico = new ERDatosBasicoCuenta();
                eRDatosBasico.Usuario = usuario;
                //   eRDatosBasico.NombreGestion = gestion;
                eRDatosBasico.NombreEmpresa = empresa;
                eRDatosBasico.FechaActual = DateTime.Now.ToString("dd/MM/yyyy");

                basicos.Add(eRDatosBasico);

                return basicos;
            }
            catch (Exception ex)
            {
                throw new MensageException("Ha ocurrido un error.");
            }
        }

        public List<ENota> ReporteObtenerNotaCompra(long idnota, long idempresa)
        {
            using (var esquema = GetEsquema())
            {

                try
                {

                    List<ENota> notas = new List<ENota>();

                    var nota = (from x in esquema.Nota
                                where x.IdEmpresa == idempresa
                                && x.TipoNota == (int)TipoNota.NotaCompra
                                && x.IdNota == idnota
                                select x).FirstOrDefault();

                    ENota enota = new ENota();

                    enota.IdNota = nota.IdNota;
                    enota.NroNota = nota.NroNota;
                    enota.Descripcion = nota.Descripcion;
                    enota.Fecha = nota.Fecha.ToString("dd/MM/yyyy");
                    enota.Total = nota.Total;

                    if(nota.Estado == (int)EstadoNota.Activo)
                    {
                        enota.Estado = "Activo";
                    }
                    else
                    {
                        enota.Estado = "Anulado";
                    }

                    notas.Add(enota);

                    return notas;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la nota de compra");
                }

            }
        }

        public List<ERDetalleNotaCompra> ReporteListarDetalleNotaCompra(long idnota)
        {
            using (var esquema = GetEsquema())
            {

                try
                {
                    List<ERDetalleNotaCompra> detalleNotaCompra = new List<ERDetalleNotaCompra>();

                    var lotes = (from x in esquema.Lote
                                 where x.IdNota == idnota
                                 select x).ToList();

                  


                    foreach (var i in lotes)
                    {
                        ERDetalleNotaCompra d = new ERDetalleNotaCompra();
                        d.IdArticulo = i.IdArticulo;
                        d.NroLote = i.NroLote;
                        d.NombreArticulo = i.Articulo.Nombre;
                        d.Cantidad = i.Cantidad;
                        d.Precio = i.PrecioCompra;
                        d.FechaVencimiento = i.FechaVencimiento != null ? i.FechaVencimiento.Value.ToString("dd/MM/yyyy"):"" ;
                        d.SubTotal = Math.Round((d.Cantidad * d.Precio), 2);
                        detalleNotaCompra.Add(d);
                    }

                    return detalleNotaCompra;
                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo listar el detalle de la nota de compra");
                }

            }

        }

    }
}
