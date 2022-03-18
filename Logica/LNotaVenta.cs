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
    public class LNotaVenta: LLogica<Nota>
    {
        public List<Nota> ListarNotaVenta(long idempresa)
        {
            using (var esquema = GetEsquema())
            {

                try
                {


                    var notas = (from x in esquema.Nota
                                 where x.IdEmpresa == idempresa
                                 && x.TipoNota == (int)TipoNota.NotaVenta
                                 orderby x.Fecha descending
                                 select x).ToList();



                    return notas;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la lista de notas de venta");
                }

            }
        }

        public long ObtenerNroNotaVenta(long idempresa)
        {
            using (var esquema = GetEsquema())
            {

                try
                {

                    long NroNota = 1;




                    var nota = (from x in esquema.Nota
                                where x.IdEmpresa == idempresa
                                && x.TipoNota == (int)TipoNota.NotaVenta
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
                    throw new MensageException("Error no se puedo obtener la nota de venta");
                }

            }
        }

        public List<Lote> ListarLote(long idarticulo)
        {
            using (var esquema = GetEsquema())
            {

                try
                {

                   


                    var lote = (from x in esquema.Lote
                                where x.IdArticulo == idarticulo
                                && x.Estado == (int)EstadoNota.Activo
                                && x.Stock > 0
                                select x).ToList();


                    return lote;

                  

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo listar el lote del articulo");
                }

            }
        }

        public Lote ObtenerLote(long idarticulo, long nrolote)
        {
            using (var esquema = GetEsquema())
            {

                try
                {




                    var lote = (from x in esquema.Lote
                                where x.IdArticulo == idarticulo
                                && x.NroLote == nrolote
                                && x.Estado == (int)EstadoLote.Activo
                                && x.Stock > 0
                                select x).FirstOrDefault();


                    if(lote != null)
                    {
                        return lote;
                    }
                    else
                    {
                        throw new MensageException("Error no se puedo obtener el lote del articulo");
                    }



                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener el lote del articulo");
                }

            }
        }


        public Nota RegistroNotaVenta(Nota Entidad, List<EDetalleNotaVenta> detalleNotaVenta, EDetalleNotaVentaTotal total)
        {
            using (var esquema = GetEsquema())
            {
                using (var transaction = new TransactionScope())
                {

                    try
                    {
                        if (detalleNotaVenta.Count == 0)
                        {
                            throw new MensageException("Ingrese el detalle de la nota de venta");
                        }


                   
                        var empresa = LEmpresa.Logica.LEmpresa.obtenerEmpresa(Entidad.IdEmpresa);


                        if (empresa.Integracion == (int)Integracion.Si)
                        {

                            List<EDetalleComprobante> detalle = new List<EDetalleComprobante>();
                            EDetalleTotal totalcomprobante = new EDetalleTotal();


                            var cuentacaja = LCuenta.Logica.LCuenta.obtenerCuenta(empresa.IdCuentaCaja.Value);
                            var cuentadebitofiscal = LCuenta.Logica.LCuenta.obtenerCuenta(empresa.IdCuentaDebitoFiscal.Value);
                            var cuentait = LCuenta.Logica.LCuenta.obtenerCuenta(empresa.IdCuentaIt.Value);
                            var cuentaventas = LCuenta.Logica.LCuenta.obtenerCuenta(empresa.IdCuentaVentas.Value);
                            var cuentaitpagar = LCuenta.Logica.LCuenta.obtenerCuenta(empresa.IdCuentaItPorPagar.Value);
                            

                            double CuentaCajaTotal = total.Total;
                            double CuentaItTotal = Math.Round((CuentaCajaTotal * 0.03), 2);
                            double CuentaDebitoFiscalTotal = Math.Round((CuentaCajaTotal * 0.13), 2);
                            double CuentaVentaTotal = Math.Round(( CuentaCajaTotal - CuentaDebitoFiscalTotal ), 2);
                            double CuentaItPagarTotal = Math.Round((CuentaCajaTotal * 0.03), 2);
                       


                            EDetalleComprobante CuentaCaja = new EDetalleComprobante();
                            CuentaCaja.IdCuenta = cuentacaja.idCuenta;
                            CuentaCaja.Cuenta = cuentacaja.Codigo + " " + cuentacaja.Nombre;
                            CuentaCaja.Glosa = "Venta de Mercaderias";
                            CuentaCaja.Debe = CuentaCajaTotal;
                            CuentaCaja.Haber = 0;
                            detalle.Add(CuentaCaja);


                            EDetalleComprobante CuentaIt = new EDetalleComprobante();
                            CuentaIt.IdCuenta = cuentait.idCuenta;
                            CuentaIt.Cuenta = cuentait.Codigo + " " + cuentait.Nombre;
                            CuentaIt.Glosa = "Venta de Mercaderias";
                            CuentaIt.Debe = CuentaItTotal;
                            CuentaIt.Haber = 0;
                            detalle.Add(CuentaIt);

                            EDetalleComprobante CuentaVentas = new EDetalleComprobante();
                            CuentaVentas.IdCuenta = cuentaventas.idCuenta;
                            CuentaVentas.Cuenta = cuentaventas.Codigo + " " + cuentaventas.Nombre;
                            CuentaVentas.Glosa = "Venta de Mercaderias";
                            CuentaVentas.Debe = 0;
                            CuentaVentas.Haber = CuentaVentaTotal;
                            detalle.Add(CuentaVentas);

                            EDetalleComprobante CuentaDebitoFiscal = new EDetalleComprobante();
                            CuentaDebitoFiscal.IdCuenta = cuentadebitofiscal.idCuenta;
                            CuentaDebitoFiscal.Cuenta = cuentadebitofiscal.Codigo + " " + cuentadebitofiscal.Nombre;
                            CuentaDebitoFiscal.Glosa = "Venta de Mercaderias";
                            CuentaDebitoFiscal.Debe = 0;
                            CuentaDebitoFiscal.Haber = CuentaDebitoFiscalTotal;
                            detalle.Add(CuentaDebitoFiscal);

                            EDetalleComprobante CuentaItPagar = new EDetalleComprobante();
                            CuentaItPagar.IdCuenta = cuentaitpagar.idCuenta;
                            CuentaItPagar.Cuenta = cuentaitpagar.Codigo + " " + cuentaitpagar.Nombre;
                            CuentaItPagar.Glosa = "Venta de Mercaderias";
                            CuentaItPagar.Debe = 0;
                            CuentaItPagar.Haber = CuentaItPagarTotal;
                            detalle.Add(CuentaItPagar);


                            totalcomprobante.TotalDebe = Math.Round((CuentaCaja.Debe + CuentaIt.Debe), 2);
                            totalcomprobante.TotalHaber = Math.Round((CuentaVentas.Haber + CuentaDebitoFiscal.Haber + CuentaItPagar.Haber), 2);


                            var moneda = LMoneda.Logica.LMoneda.obtenerMonedadXempresa(empresa.idEmpresa);
                            Comprobante ComprobanteEntidad = new Comprobante()
                            {
                                Serie = LComprobante.Logica.LComprobante.obtenerSerie(empresa.idEmpresa),
                                Glosa = "Venta de Mercaderias",
                                Fecha = Entidad.Fecha,
                                TipoCambio = moneda.Cambio.Value,
                                Estado = (int)EstadoComprobante.Abierto,
                                TipoComprobante = (int)TipoComprobante.Ingreso,
                                IdEmpresa = empresa.idEmpresa,
                                IdMoneda = moneda.IdMonedaPrincipal,
                                IdUsuario = Entidad.IdUsuario

                            };

                            ComprobanteEntidad = LComprobante.Logica.LComprobante.Registro(ComprobanteEntidad, detalle, totalcomprobante);

                            Entidad.IdComprobante = ComprobanteEntidad.IdComprobante;

                        }



                        Entidad.NroNota = ObtenerNroNotaVenta(Entidad.IdEmpresa);
                        Entidad.Total = total.Total;
                        esquema.Nota.Add(Entidad);
                        esquema.SaveChanges();

                        foreach (var i in detalleNotaVenta)
                        {
                            DetalleVenta detalleVenta = new DetalleVenta();
                            detalleVenta.IdArticulo = i.IdArticulo;
                            detalleVenta.NroLote = i.NroLote;
                            detalleVenta.IdNota = Entidad.IdNota;
                            detalleVenta.Cantidad = i.Cantidad;
                            detalleVenta.PrecioVenta = i.Precio;

                            var lote = (from x in esquema.Lote
                                        where x.IdArticulo == i.IdArticulo
                                        && x.NroLote == i.NroLote
                                        select x).FirstOrDefault();

                            if(lote == null)
                            {
                                throw new MensageException("Error al obtener el lote");
                            }

                           
                            
                            if(i.Cantidad > lote.Stock)
                            {
                                throw new MensageException("No hay suficiente stock en el lote " + lote.NroLote + " para el articulo " + i.NombreArticulo);
                            }

                            lote.Stock = lote.Stock - i.Cantidad;
                            lote.Articulo.Cantidad = lote.Articulo.Cantidad - i.Cantidad;

                            if(lote.Stock == 0)
                            {
                                lote.Estado = (int)EstadoLote.SinStock;
                            }

                            esquema.SaveChanges();

                            esquema.DetalleVenta.Add(detalleVenta);
                            esquema.SaveChanges();

                        }

                        transaction.Complete();
                        return Entidad;


                    }
                    catch (MensageException ex)
                    {
                        throw new MensageException(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        throw new MensageException("Error al registrar la nota de venta");
                    }
                }
            }
        }


        public Nota ObtenerNotaVenta(long idnota, long idempresa)
        {
            using (var esquema = GetEsquema())
            {

                try
                {

                    var nota = (from x in esquema.Nota
                                where x.IdEmpresa == idempresa
                                && x.TipoNota == (int)TipoNota.NotaVenta
                                && x.IdNota == idnota
                                select x).FirstOrDefault();

                    if (nota != null)
                    {
                        return nota;

                    }
                    else
                    {
                        throw new MensageException("Error no se puedo obtener la nota de venta");
                    }



                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la nota de venta");
                }

            }
        }

        public List<EDetalleNotaVenta> ListarDetalleNotaVenta(long idnota)
        {
            using (var esquema = GetEsquema())
            {

                try
                {

                    var detalleventa = (from x in esquema.DetalleVenta
                                 where x.IdNota == idnota
                                 select x).ToList();

                    List<EDetalleNotaVenta> detalle = new List<EDetalleNotaVenta>();


                    foreach (var i in detalleventa)
                    {
                        EDetalleNotaVenta d = new EDetalleNotaVenta();
                        d.IdArticulo = i.IdArticulo;
                        d.NroLote = i.NroLote;
                        d.NombreArticulo = i.Articulo.Nombre;
                        d.Cantidad = i.Cantidad;
                        d.Precio = i.PrecioVenta;
                        d.SubTotal = Math.Round((d.Cantidad * d.Precio), 2);
                        detalle.Add(d);
                    }

                    return detalle;
                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo listar el detalle de la nota de venta");
                }

            }

        }


        public Nota AnularNotaVenta(long idnota, long idempresa)
        {
            using (var esquema = GetEsquema())
            {

                try
                {

                    var nota = (from x in esquema.Nota
                                where x.IdEmpresa == idempresa
                                && x.TipoNota == (int)TipoNota.NotaVenta
                                && x.IdNota == idnota
                                select x).FirstOrDefault();

                    if (nota != null)
                    {
                        if(nota.Comprobante != null)
                        {
                            nota.Comprobante.Estado = (int)EstadoComprobante.Anualdo;
                        }

                        nota.Estado = (int)EstadoNota.Anulado;

                        foreach (var i in nota.DetalleVenta)
                        {

                            var lote = (from x in esquema.Lote
                                            where x.IdArticulo == i.IdArticulo
                                            && x.NroLote == i.NroLote
                                            select x).FirstOrDefault();


                            // i.Estado = (int)EstadoNota.Anulado;

                            lote.Estado = (int)EstadoLote.Activo;

                            lote.Stock = lote.Stock + i.Cantidad;
                            lote.Articulo.Cantidad = lote.Articulo.Cantidad + i.Cantidad;
                            esquema.SaveChanges();
                        }

                        esquema.SaveChanges();

                        return nota;

                    }
                    else
                    {
                        throw new MensageException("Error no se puedo obtener la nota de venta");
                    }



                }
                catch(MensageException ex)
                {
                    throw new MensageException(ex.Message);
                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la nota de venta");
                }

            }
        }

        public List<ERDatosBasicoCuenta> ReporteDatosBasicoNotaVenta(string usuario, string empresa)
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

        public List<ENota> ReporteObtenerNotaVenta(long idnota, long idempresa)
        {
            using (var esquema = GetEsquema())
            {

                try
                {

                    List<ENota> notas = new List<ENota>();

                    var nota = (from x in esquema.Nota
                                where x.IdEmpresa == idempresa
                                && x.TipoNota == (int)TipoNota.NotaVenta
                                && x.IdNota == idnota
                                select x).FirstOrDefault();

                    ENota enota = new ENota();

                    enota.IdNota = nota.IdNota;
                    enota.NroNota = nota.NroNota;
                    enota.Descripcion = nota.Descripcion;
                    enota.Fecha = nota.Fecha.ToString("dd/MM/yyyy");
                    enota.Total = nota.Total;

                    if (nota.Estado == (int)EstadoNota.Activo)
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


        public List<ERDetalleNotaVenta> ReporteListarDetalleNotaVenta(long idnota)
        {
            using (var esquema = GetEsquema())
            {

                try
                {

                    var detalleventa = (from x in esquema.DetalleVenta
                                        where x.IdNota == idnota
                                        select x).ToList();

                    List<ERDetalleNotaVenta> detalle = new List<ERDetalleNotaVenta>();


                    foreach (var i in detalleventa)
                    {
                        ERDetalleNotaVenta d = new ERDetalleNotaVenta();
                        d.IdArticulo = i.IdArticulo;
                        d.NroLote = i.NroLote;
                        d.NombreArticulo = i.Articulo.Nombre;
                        d.Cantidad = i.Cantidad;
                        d.Precio = i.PrecioVenta;
                        d.SubTotal = Math.Round((d.Cantidad * d.Precio), 2);
                        detalle.Add(d);
                    }

                    return detalle;
                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo listar el detalle de la nota de venta");
                }

            }

        }

    }
}
