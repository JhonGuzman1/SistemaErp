using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Logica;
using Entidad;
using Datos;
using Entidad.Estados;
using Microsoft.Reporting.WebForms;

namespace SistemaERP.Controllers
{
    public class NotaVentaController : Controller
    {
        private LNotaVenta logica = LNotaVenta.Logica.LNotaVenta;
        public ActionResult Index()
        {

            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];

                return View(logica.ListarNotaVenta(empresa.idEmpresa));
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('Ha ocurrido un error.');");
            }
        }

        public ActionResult NotaVenta()
        {

            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];

                Session["DetalleNotaVenta"] = new List<EDetalleNotaVenta>();
                ViewBag.Articulo = LArticulo.Logica.LArticulo.listarArticulo(empresa.idEmpresa);
                ViewBag.NroNotaVenta = logica.ObtenerNroNotaVenta(empresa.idEmpresa);
                

                EDetalleNotaVentaVer d = new EDetalleNotaVentaVer();
                d.Ver = 0;
                Session["NotaVentaVer"] = d;

                EDetalleNotaVentaTotal e = new EDetalleNotaVentaTotal();
                e.Total = 0;


                Session["DetalleTotalVenta"] = e;

                return View();
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('Ha ocurrido un error.');");
            }
        }

        public ActionResult ListarLote(long idarticulo)
        {


            try
            {


                return PartialView("ListarLote", logica.ListarLote(idarticulo));

            }
            catch (MensageException ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }

            // return PartialView("ListaDetalleComprobante", detalle);
        }

        public ActionResult ListarArticulo(long idarticulo)
        {


            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];

                return PartialView("ListarArticulo", LArticulo.Logica.LArticulo.obtenerArticulo1(idarticulo,empresa.idEmpresa));

            }
            catch (MensageException ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }

            // return PartialView("ListaDetalleComprobante", detalle);
        }


        public ActionResult ListarDetalleNotaVenta()
        {

            List<EDetalleNotaVenta> detalle = new List<EDetalleNotaVenta>();
            EDetalleNotaVentaTotal total = new EDetalleNotaVentaTotal();

            try
            {


                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];


                detalle = Session["DetalleNotaVenta"] as List<EDetalleNotaVenta>;
                total = Session["DetalleTotalVenta"] as EDetalleNotaVentaTotal;



                return PartialView("ListaDetalleNotaVenta", detalle);

            }
            catch (MensageException ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }

            // return PartialView("ListaDetalleComprobante", detalle);
        }

        public ActionResult RegistroDetalleNotaVenta(long idarticulo,long nrolote, int cantidad, double precio)
        {

          

            List<EDetalleNotaVenta> detalle = new List<EDetalleNotaVenta>();
            EDetalleNotaVentaTotal total = new EDetalleNotaVentaTotal();

            try
            {


                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];


                detalle = Session["DetalleNotaVenta"] as List<EDetalleNotaVenta>;
                // total = Session["DetalleTotal"] as EDetalleTotal;

                var articulo = LArticulo.Logica.LArticulo.obtenerArticulo1(idarticulo, empresa.idEmpresa);
                var lote = logica.ObtenerLote(idarticulo, nrolote);

                

                /*   if (Int32.)
                   {
                       throw new MensageException("Debe ingresar un valor");
                   }*/

               /* if (Double.IsNaN(precio))
                {
                    throw new MensageException("Debe ingresar un precio");
                }*/

                if(cantidad <= 0)
                {
                    throw new MensageException("La cantidad debe ser mayor a 0");
                }

                if (cantidad > lote.Stock)
                {
                    throw new MensageException("No hay suficiente stock en el lote "+lote.NroLote+" para el articulo "+articulo.Nombre );
                }


                if (detalle.Count == 0)
                {
                    EDetalleNotaVenta d = new EDetalleNotaVenta();
                    d.IdArticulo = idarticulo;
                    d.NroLote = nrolote;
                    d.NombreArticulo = articulo.Nombre;
                    d.Cantidad = cantidad;
                    d.Precio = precio;
                    d.SubTotal = Math.Round((d.Cantidad * d.Precio), 2);
                  
                    detalle.Add(d);

                    total.Total = d.SubTotal;

                }
                else if (detalle.Count > 0)
                {

                    foreach (var i in detalle)
                    {
                        if (articulo.IdArticulo == i.IdArticulo && nrolote == i.NroLote)
                        {
                            throw new MensageException("El articulo y el lote ya existe en el listado");
                        }

                        total.Total = Math.Round((total.Total + i.SubTotal), 2);


                    }

                    EDetalleNotaVenta d = new EDetalleNotaVenta();
                    d.IdArticulo = idarticulo;
                    d.NroLote = nrolote;
                    d.NombreArticulo = articulo.Nombre;
                    d.Cantidad = cantidad;
                    d.Precio = precio;
                    d.SubTotal = Math.Round((d.Cantidad * d.Precio), 2);
                   
                    detalle.Add(d);

                    total.Total = Math.Round((total.Total + d.SubTotal), 2);


                }



                Session["DetalleNotaVenta"] = detalle;
                Session["DetalleTotalVenta"] = total;

                string hola = "hola";
                return JavaScript("MensajeExitoDetalle('" + hola + "');");
            }
            catch (MensageException ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }

            // return PartialView("ListaDetalleComprobante", detalle);
        }

        public ActionResult EditarDetalleNotaVenta(long idarticulo, int cantidad, double precio, long nrolote)
        {

            List<EDetalleNotaVenta> detalle = new List<EDetalleNotaVenta>();
            EDetalleNotaVentaTotal total = new EDetalleNotaVentaTotal();

            try
            {


                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];


                detalle = Session["DetalleNotaVenta"] as List<EDetalleNotaVenta>;
                // total = Session["DetalleTotal"] as EDetalleTotal;

                var articulo = LArticulo.Logica.LArticulo.obtenerArticulo1(idarticulo, empresa.idEmpresa);

                var lote = logica.ObtenerLote(idarticulo, nrolote);

                if (cantidad <= 0)
                {
                    throw new MensageException("La cantidad debe ser mayor a 0");
                }

                if (cantidad > lote.Stock)
                {
                    throw new MensageException("No hay suficiente stock en el lote " + lote.NroLote + " para el articulo " + articulo.Nombre);
                }

                int co = 0;

                foreach (var i in detalle)
                {
                    if (articulo.IdArticulo == i.IdArticulo && i.NroLote == nrolote)
                    {

                        i.NombreArticulo = articulo.Nombre;
                        i.Cantidad = cantidad;
                        i.Precio = precio;
                        i.SubTotal = Math.Round((i.Cantidad * i.Precio), 2);

                        co = co + 1;
                    }
                  /*  else
                    {
                        throw new MensageException("El lote y el articulo no existe en el listado");
                    }*/

                    total.Total = Math.Round((total.Total + i.SubTotal), 2);


                }

                if(co == 0)
                {
                    throw new MensageException("El lote o el articulo no existe en el listado");
                }


                Session["DetalleNotaVenta"] = detalle;
                Session["DetalleTotalVenta"] = total;

                string hola = "editar exito";
                return JavaScript("MensajeExitoDetalle('" + hola + "');");
            }
            catch (MensageException ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }

            // return PartialView("ListaDetalleComprobante", detalle);
        }

        [HttpPost]
        public ActionResult EliminarDetalleNotaVenta(long idarticulo, long nrolote)
        {


            try
            {


                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];

                List<EDetalleNotaVenta> detalle = Session["DetalleNotaVenta"] as List<EDetalleNotaVenta>;
                detalle.RemoveAll(p => p.IdArticulo == idarticulo && p.NroLote == nrolote);
                EDetalleNotaVentaTotal total = new EDetalleNotaVentaTotal();
                total.Total = 0;

                foreach (var i in detalle)
                {

                    total.Total = Math.Round((total.Total + i.SubTotal), 2);


                }


                Session["DetalleNotaVenta"] = detalle;
                Session["DetalleTotalVenta"] = total;

                string hola = "registro exito";
                return JavaScript("MensajeEliminarDetalle('" + hola + "');");
            }
            catch (MensageException ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }

            // return PartialView("ListaDetalleComprobante", detalle);
        }


        [HttpPost]
        public ActionResult Registro(string fecha, string descripcion)
        {

            try
            {
                DateTime FechaNota;
                try
                {
                    FechaNota = DateTime.Parse(fecha);

                }
                catch (Exception ex)
                {
                    throw new MensageException("Ingrese la fecha con formato valido");
                }



                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];


                Nota Entidad = new Nota()
                {
                    Fecha = FechaNota,
                    Descripcion = descripcion,
                    Estado = (int)EstadoNota.Activo,
                    TipoNota = (int)TipoNota.NotaVenta,
                    IdEmpresa = empresa.idEmpresa,
                    IdUsuario = usuario.idUsuario

                };

                List<EDetalleNotaVenta> detalle = Session["DetalleNotaVenta"] as List<EDetalleNotaVenta>;
                EDetalleNotaVentaTotal total = Session["DetalleTotalVenta"] as EDetalleNotaVentaTotal;
                Entidad = logica.RegistroNotaVenta(Entidad, detalle, total);

             /*   return JavaScript("redireccionar('" + Url.Action("Index", "NotaVenta") + "');");*/
                // return View("FormularioConductor", entidad);
               return JavaScript("redireccionar('" + Url.Action("ENotaVenta", "NotaVenta", new { idnota = Entidad.IdNota }) + "');");

            }
            catch (MensageException ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }

        }


        public ActionResult ENotaVenta(long idnota)
        {

            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];

                Nota nota = logica.ObtenerNotaVenta(idnota, empresa.idEmpresa);

                List<EDetalleNotaVenta> detalleNotaVentas = new List<EDetalleNotaVenta>();
                detalleNotaVentas = logica.ListarDetalleNotaVenta(nota.IdNota);

                Session["DetalleNotaVenta"] = detalleNotaVentas;

                EDetalleNotaVentaVer d = new EDetalleNotaVentaVer();
                d.Ver = 1;
                Session["NotaVentaVer"] = d;

                EDetalleNotaVentaTotal e = new EDetalleNotaVentaTotal();
                e.Total = nota.Total;


                Session["DetalleTotalVenta"] = e;

                return View(nota);
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('Ha ocurrido un error.');");
            }
        }

        [HttpPost]
        public ActionResult Anular(long idnota)
        {

            try
            {


                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];




                Nota Entidad = logica.AnularNotaVenta(idnota, empresa.idEmpresa);

                return JavaScript("redireccionar('" + Url.Action("ENotaVenta", "NotaVenta", new { idnota = Entidad.IdNota }) + "');");

            }
            catch (MensageException ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }

        }

        public ActionResult ReporteNotaVenta(long idnota)
        {
            try
            {


                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];

                List<ERDatosBasicoCuenta> datosBasico = new List<ERDatosBasicoCuenta>();
                datosBasico = logica.ReporteDatosBasicoNotaVenta(usuario.Usuario1, empresa.Nombre);


                List<ENota> nota = new List<ENota>();
                nota = logica.ReporteObtenerNotaVenta(idnota, empresa.idEmpresa);

                List<ERDetalleNotaVenta> detalleComprobantes = new List<ERDetalleNotaVenta>();
                detalleComprobantes = logica.ReporteListarDetalleNotaVenta(idnota);



                ReportViewer viewer = new ReportViewer();
                viewer.AsyncRendering = false;
                viewer.SizeToReportContent = true;

                ReportDataSource rb = new ReportDataSource("DSReporteBasicoNota", datosBasico);
                ReportDataSource rp = new ReportDataSource("DSReporteNota", nota);
                ReportDataSource rcdetalle = new ReportDataSource("DSReporteDetalleNota", detalleComprobantes);

                viewer.LocalReport.ReportPath = Server.MapPath("~/Reportes/ReporteNotaVenta.rdlc");
                viewer.LocalReport.DataSources.Clear();
                viewer.LocalReport.DataSources.Add(rp);
                viewer.LocalReport.DataSources.Add(rb);
                viewer.LocalReport.DataSources.Add(rcdetalle);

                viewer.LocalReport.Refresh();

                ViewBag.ReporteNotaVenta = viewer;
                ViewBag.NotaVenta = nota;

                return View("ReporteNotaVenta");
            }
            catch (MensageException ex)
            {
                string mensaje = ex.Message.Replace("'", "");
                ViewBag.Mensaje = mensaje;
                return JavaScript("MostrarMensaje('" + mensaje + "');");
            }
            catch (Exception ex)
            {

                return JavaScript("MostrarMensaje('Ha ocurrido un error');");
            }

        }


    }
}
