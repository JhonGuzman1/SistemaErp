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
    public class NotaCompraController : Controller
    {
        private LNotaCompra logica = LNotaCompra.Logica.LNotaCompra;
        public ActionResult Index()
        {

            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];

                return View(logica.listarNotaCompra(empresa.idEmpresa));
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('Ha ocurrido un error.');");
            }
        }

        public ActionResult NotaCompra()
        {

            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];

                Session["DetalleNotaCompra"] = new List<EDetalleNotaCompra>();
                ViewBag.Articulo = LArticulo.Logica.LArticulo.listarArticulo(empresa.idEmpresa);
                ViewBag.NroNota = logica.obtenerNroNotaCompra(empresa.idEmpresa);

                EDetalleNotaCompraVer d = new EDetalleNotaCompraVer();
                d.Ver = 0;
                Session["NotaCompraVer"] = d;

                EDetalleNotaCompraTotal e = new EDetalleNotaCompraTotal();
                e.Total = 0;
                

                Session["DetalleTotalCompra"] = e;

                return View();
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('Ha ocurrido un error.');");
            }
        }

        public ActionResult ListarDetalleNotaCompra()
        {

            List<EDetalleNotaCompra> detalle = new List<EDetalleNotaCompra>();
            EDetalleNotaCompraTotal total = new EDetalleNotaCompraTotal();

            try
            {


                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];


                detalle = Session["DetalleNotaCompra"] as List<EDetalleNotaCompra>;
                total = Session["DetalleTotalCompra"] as EDetalleNotaCompraTotal;



                return PartialView("ListaDetalleNotaCompra", detalle);

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

        public ActionResult RegistroDetalleNotaCompra(long idarticulo, int cantidad, double precio,string fechavencimiento)
        {

            try
            {

                if (!string.IsNullOrEmpty(fechavencimiento))
                {
                    DateTime FechaInicio = DateTime.Parse(fechavencimiento);
                }

             
           
            }
            catch (Exception ex)
            {
                throw new MensageException("Ingrese las fechas con formato valido");
            }

            List<EDetalleNotaCompra> detalle = new List<EDetalleNotaCompra>();
            EDetalleNotaCompraTotal total = new EDetalleNotaCompraTotal();

            try
            {


                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];


                detalle = Session["DetalleNotaCompra"] as List<EDetalleNotaCompra>;
                // total = Session["DetalleTotal"] as EDetalleTotal;

                var articulo = LArticulo.Logica.LArticulo.obtenerArticulo1(idarticulo,empresa.idEmpresa);

             /*   if (Int32.)
                {
                    throw new MensageException("Debe ingresar un valor");
                }*/

                if (Double.IsNaN(precio))
                {
                    throw new MensageException("Debe ingresar un precio");
                }



                if (detalle.Count == 0)
                {
                    EDetalleNotaCompra d = new EDetalleNotaCompra();
                    d.IdArticulo = idarticulo;
                    d.NombreArticulo = articulo.Nombre;
                    d.Cantidad = cantidad;
                    d.Precio = precio;
                    d.SubTotal = Math.Round((d.Cantidad * d.Precio), 2) ;
                    if (!string.IsNullOrEmpty(fechavencimiento))
                    {
                       d.FechaVencimiento = DateTime.Parse(fechavencimiento);
                    }
                    detalle.Add(d);

                    total.Total = d.SubTotal;

                }
                else if (detalle.Count > 0)
                {

                    foreach (var i in detalle)
                    {
                        if (articulo.IdArticulo == i.IdArticulo)
                        {
                            throw new MensageException("El articulo ya existe en el listado");
                        }

                        total.Total = Math.Round((total.Total + i.SubTotal), 2);
                       

                    }

                    EDetalleNotaCompra d = new EDetalleNotaCompra();
                    d.IdArticulo = idarticulo;
                    d.NombreArticulo = articulo.Nombre;
                    d.Cantidad = cantidad;
                    d.Precio = precio;
                    d.SubTotal = Math.Round((d.Cantidad * d.Precio), 2) ;
                    if (!string.IsNullOrEmpty(fechavencimiento))
                    {
                        d.FechaVencimiento = DateTime.Parse(fechavencimiento);
                    }
                    detalle.Add(d);

                    total.Total = Math.Round((total.Total + d.SubTotal), 2);

                 
                }


            
              

                Session["DetalleNotaCompra"] = detalle;
                Session["DetalleTotalCompra"] = total;

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

        public ActionResult EditarDetalleNotaCompra(long idarticulo, int cantidad, double precio, string fechavencimiento)
        {

            List<EDetalleNotaCompra> detalle = new List<EDetalleNotaCompra>();
            EDetalleNotaCompraTotal total = new EDetalleNotaCompraTotal();

            try
            {


                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];


                detalle = Session["DetalleNotaCompra"] as List<EDetalleNotaCompra>;
                // total = Session["DetalleTotal"] as EDetalleTotal;

                var articulo = LArticulo.Logica.LArticulo.obtenerArticulo1(idarticulo,empresa.idEmpresa);


                /*int cuentaRepetidas = 0;

                foreach (var i in detalle)
                {
                    if (i.IdArticulo == idarticulo)
                    {
                        cuentaRepetidas = cuentaRepetidas + 1;
                    }
                }

                if (cuentaRepetidas >= 2)
                {
                    throw new MensageException("El articulo ya existe en el listado");
                }*/


                foreach (var i in detalle)
                {
                    if (articulo.IdArticulo == i.IdArticulo)
                    {
                      
                        i.NombreArticulo = articulo.Nombre;
                        i.Cantidad = cantidad;
                        i.Precio = precio;
                        i.SubTotal = Math.Round((i.Cantidad * i.Precio), 2);
                        if (!string.IsNullOrEmpty(fechavencimiento))
                        {
                            i.FechaVencimiento = DateTime.Parse(fechavencimiento);
                        }
                        else
                        {
                            i.FechaVencimiento = null;
                        }
                    }

                    total.Total = Math.Round((total.Total + i.SubTotal), 2);


                }



                Session["DetalleNotaCompra"] = detalle;
                Session["DetalleTotalCompra"] = total;

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
        public ActionResult EliminarDetalleNotaCompra(long idarticulo)
        {


            try
            {


                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];

                List<EDetalleNotaCompra> detalleComprobantes = Session["DetalleNotaCompra"] as List<EDetalleNotaCompra>;
                detalleComprobantes.RemoveAll(p => p.IdArticulo == idarticulo);
                EDetalleNotaCompraTotal total = new EDetalleNotaCompraTotal();
                total.Total = 0;
               
                foreach (var i in detalleComprobantes)
                {

                    total.Total = Math.Round((total.Total + i.SubTotal), 2);


                }


                Session["DetalleNotaCompra"] = detalleComprobantes;
                Session["DetalleTotalCompra"] = total;

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
                    TipoNota = (int)TipoNota.NotaCompra,
                    IdEmpresa = empresa.idEmpresa,
                    IdUsuario = usuario.idUsuario

                };

                List<EDetalleNotaCompra> detalle = Session["DetalleNotaCompra"] as List<EDetalleNotaCompra>;
                EDetalleNotaCompraTotal total = Session["DetalleTotalCompra"] as EDetalleNotaCompraTotal;
                Entidad = logica.RegistroNotaCompra(Entidad, detalle, total);

              /*  return JavaScript("redireccionar('" + Url.Action("Index", "NotaCompra") + "');");*/
                // return View("FormularioConductor", entidad);
                  return JavaScript("redireccionar('" + Url.Action("ENotaCompra", "NotaCompra", new { idnota = Entidad.IdNota }) + "');");

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

        public ActionResult ENotaCompra(long idnota)
        {

            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];

                Nota nota = logica.ObtenerNotaCompra(idnota, empresa.idEmpresa);
               
                List<EDetalleNotaCompra> detalleNotaCompras = new List<EDetalleNotaCompra>();
                detalleNotaCompras = logica.ListarDetalleNotaCompra(nota.IdNota);

                Session["DetalleNotaCompra"] = detalleNotaCompras;

                EDetalleNotaCompraVer d = new EDetalleNotaCompraVer();
                d.Ver = 1;
                Session["NotaCompraVer"] = d;

                EDetalleNotaCompraTotal e = new EDetalleNotaCompraTotal();
                e.Total = nota.Total;


                Session["DetalleTotalCompra"] = e;

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




               Nota Entidad =  logica.AnularNotaCompra(idnota,empresa.idEmpresa);

                return JavaScript("redireccionar('" + Url.Action("ENotaCompra", "NotaCompra", new { idnota = Entidad.IdNota }) + "');");

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

        public ActionResult ReporteNotaCompra(long idnota)
        {
            try
            {


                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];

                List<ERDatosBasicoCuenta> datosBasico = new List<ERDatosBasicoCuenta>();
                datosBasico = logica.ReporteDatosBasicoNotaCompra(usuario.Usuario1, empresa.Nombre);


                List<ENota> nota = new List<ENota>();
                nota = logica.ReporteObtenerNotaCompra(idnota, empresa.idEmpresa);

                List<ERDetalleNotaCompra> detalleComprobantes = new List<ERDetalleNotaCompra>();
                detalleComprobantes = logica.ReporteListarDetalleNotaCompra(idnota);

              

                ReportViewer viewer = new ReportViewer();
                viewer.AsyncRendering = false;
                viewer.SizeToReportContent = true;

                ReportDataSource rb = new ReportDataSource("DSReporteBasicoNota", datosBasico);
                ReportDataSource rp = new ReportDataSource("DSReporteNota", nota);
                ReportDataSource rcdetalle = new ReportDataSource("DSReporteDetalleNota", detalleComprobantes);

                viewer.LocalReport.ReportPath = Server.MapPath("~/Reportes/ReporteNotaCompra.rdlc");
                viewer.LocalReport.DataSources.Clear();
                viewer.LocalReport.DataSources.Add(rp);
                viewer.LocalReport.DataSources.Add(rb);
                viewer.LocalReport.DataSources.Add(rcdetalle);
             
                viewer.LocalReport.Refresh();

                ViewBag.ReporteNotaCompra = viewer;
                ViewBag.NotaCompra = nota;

                return View("ReporteNotaCompra");
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
