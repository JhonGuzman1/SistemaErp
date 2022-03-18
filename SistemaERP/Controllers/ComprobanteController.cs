using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entidad;
using Logica;
using Datos;
using Entidad.Estados;
using Microsoft.Reporting.WebForms;

namespace SistemaERP.Controllers
{
    public class ComprobanteController : Controller
    {
        private LComprobante logica = LComprobante.Logica.LComprobante;
        public ActionResult Index()
        {

            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];
             
                return View(logica.listarComprobante(empresa.idEmpresa));
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('Ha ocurrido un error.');");
            }
        }


        public ActionResult Comprobante()
        {

            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];
                
                Session["DetalleComprobante"] = new List<EDetalleComprobante>();
                ViewBag.CuentaDetalle = LCuenta.Logica.LCuenta.listarCuentaDetalle(empresa.idEmpresa);
                ViewBag.EmpresaMonedas = LMoneda.Logica.LMoneda.listarMonedaActivaXEmpresa(empresa.idEmpresa);
                ViewBag.Serie = logica.obtenerSerie(empresa.idEmpresa);

                DetalleEstado d = new DetalleEstado();
                d.Estado = 1;
                Session["EstadoComprobante"] = d;

                EDetalleTotal e = new EDetalleTotal();
                e.TotalDebe = 0;
                e.TotalHaber = 0;

                Session["DetalleTotal"] = e;

                return View();
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('Ha ocurrido un error.');");
            }
        }

        public ActionResult EComprobante(long idcomprobante)
        {

            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];

                Comprobante comprobante = logica.ObtenerComprobante(empresa.idEmpresa, idcomprobante);
                ViewBag.CuentaDetalle = LCuenta.Logica.LCuenta.listarCuentaDetalle(empresa.idEmpresa);
                ViewBag.EmpresaMonedas = LMoneda.Logica.LMoneda.listarMonedaActivaXEmpresa(empresa.idEmpresa);

               

                List<EDetalleComprobante> detalleComprobantes = new List<EDetalleComprobante>();
                detalleComprobantes = logica.listarDetalleComprobanteXComprobante(comprobante.IdComprobante, comprobante.IdEmpresa);

                Session["DetalleComprobante"] = detalleComprobantes;

                //ViewBag.Serie = logica.obtenerSerie(empresa.idEmpresa);
                DetalleEstado d = new DetalleEstado();
                d.Estado = comprobante.Estado;
                Session["EstadoComprobante"] = d;

                EDetalleTotal e = new EDetalleTotal();
                e.TotalDebe = 0;
                e.TotalHaber = 0;

                foreach(var i in detalleComprobantes)
                {
                    e.TotalDebe = e.TotalDebe + i.Debe;
                    e.TotalHaber = e.TotalHaber + i.Haber;
                }

                Session["DetalleTotal"] = e;

                return View(comprobante);
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('Ha ocurrido un error.');");
            }
        }

        public ActionResult RegistroDetalleComprobante(long idcuenta,string detalleglosa, double debe, double haber)
        {

            List<EDetalleComprobante> detalle = new List<EDetalleComprobante>();
            EDetalleTotal total = new EDetalleTotal();

            try
            {


                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];
               

                detalle = Session["DetalleComprobante"] as List<EDetalleComprobante>;
               // total = Session["DetalleTotal"] as EDetalleTotal;

                var cuenta = LCuenta.Logica.LCuenta.obtenerCuenta(idcuenta);

                if (Double.IsNaN(debe))
                {
                    throw new MensageException("Debe ingresar un valor");
                }

                if (Double.IsNaN(haber))
                {
                    throw new MensageException("Debe ingresar un valor");
                }



                if (detalle.Count == 0)
                {
                    EDetalleComprobante d = new EDetalleComprobante();
                    d.IdCuenta = cuenta.idCuenta;
                    d.Cuenta = cuenta.Codigo + " " + cuenta.Nombre;
                    d.Glosa = detalleglosa;
                    d.Debe = debe;
                    d.Haber = haber;
                    detalle.Add(d);

                    total.TotalDebe = debe;
                    total.TotalHaber = haber;

                }else if(detalle.Count > 0)
                {

                    foreach(var i in detalle)
                    {
                        if(cuenta.idCuenta == i.IdCuenta)
                        {
                            throw new MensageException("La cuenta ya existe en el listado");
                        }

                        total.TotalDebe = total.TotalDebe + i.Debe;
                        total.TotalHaber = total.TotalHaber + i.Haber;

                    }

                    EDetalleComprobante d = new EDetalleComprobante();
                    d.IdCuenta = cuenta.idCuenta;
                    d.Cuenta = cuenta.Codigo + " " + cuenta.Nombre;
                    d.Glosa = detalleglosa;
                    d.Debe = debe;
                    d.Haber = haber;
                    detalle.Add(d);

                    total.TotalDebe = total.TotalDebe + debe;
                    total.TotalHaber = total.TotalHaber + haber;
                }

             
                total.TotalDebe = total.TotalDebe;
                total.TotalHaber = total.TotalHaber;

              //  ViewBag.DetalleTotal = total;

                Session["DetalleComprobante"] = detalle;
                Session["DetalleTotal"] = total;

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


        public ActionResult ListarDetalleComprobante()
        {

            List<EDetalleComprobante> detalle = new List<EDetalleComprobante>();
            EDetalleTotal total = new EDetalleTotal();

            try
            {


                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];


                detalle = Session["DetalleComprobante"] as List<EDetalleComprobante>;
                total = Session["DetalleTotal"] as EDetalleTotal;



                return PartialView("ListaDetalleComprobante", detalle);

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
        public ActionResult EliminarDetalleComprobante(long idcuenta)
        {


            try
            {


                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];

                List<EDetalleComprobante> detalleComprobantes = Session["DetalleComprobante"] as List<EDetalleComprobante>;
                detalleComprobantes.RemoveAll(p => p.IdCuenta == idcuenta);
                EDetalleTotal total = new EDetalleTotal();
                total.TotalDebe = 0;
                total.TotalHaber = 0;
                foreach(var i in detalleComprobantes)
                {

                    total.TotalDebe = total.TotalDebe + i.Debe;
                    total.TotalHaber = total.TotalHaber + i.Haber;

                }


                Session["DetalleComprobante"] = detalleComprobantes;
                Session["DetalleTotal"] = total;

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

        public ActionResult EditarDetalleComprobante(long idcuenta, string detalleglosa, double debe, double haber)
        {

            List<EDetalleComprobante> detalle = new List<EDetalleComprobante>();
            EDetalleTotal total = new EDetalleTotal();

            try
            {


                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];


                detalle = Session["DetalleComprobante"] as List<EDetalleComprobante>;
                // total = Session["DetalleTotal"] as EDetalleTotal;

                var cuenta = LCuenta.Logica.LCuenta.obtenerCuenta(idcuenta);


                int cuentaRepetidas = 0;

                foreach(var i in detalle)
                {
                    if(i.IdCuenta == idcuenta)
                    {
                        cuentaRepetidas = cuentaRepetidas + 1;
                    }
                }

                if(cuentaRepetidas >= 2)
                {
                    throw new MensageException("La cuenta ya existe en el listado");
                }     


                    foreach (var i in detalle)
                    {
                        if (cuenta.idCuenta == i.IdCuenta)
                        {
                            i.Cuenta = cuenta.Codigo + " " + cuenta.Nombre;
                            i.Debe = debe;
                            i.Haber = haber;
                            i.Glosa = detalleglosa;
                        }

                        total.TotalDebe = total.TotalDebe + i.Debe;
                        total.TotalHaber = total.TotalHaber + i.Haber;


                    }

                  
            
                Session["DetalleComprobante"] = detalle;
                Session["DetalleTotal"] = total;

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
        public ActionResult Registro(string fecha, int tipocomprobante,double cambio, long moneda,string glosa)
        {

            try
            {
                DateTime FechaComprobante;
                try
                {
                     FechaComprobante = DateTime.Parse(fecha);
        
                }
                catch (Exception ex)
                {
                    throw new MensageException("Ingrese la fecha con formato valido");
                }



                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];

                long nroserie = logica.obtenerSerie(empresa.idEmpresa);

                Comprobante Entidad = new Comprobante()
                {
                    Serie = nroserie,
                    Glosa = glosa,
                    Fecha = FechaComprobante,
                    TipoCambio = cambio,
                    Estado = (int)EstadoComprobante.Abierto,
                    TipoComprobante = tipocomprobante,
                    IdEmpresa = empresa.idEmpresa,
                    IdMoneda = moneda,
                    IdUsuario = usuario.idUsuario

                };

                List<EDetalleComprobante>  detalle = Session["DetalleComprobante"] as List<EDetalleComprobante>;
                EDetalleTotal total = Session["DetalleTotal"] as EDetalleTotal;
               Entidad = logica.Registro(Entidad, detalle, total);

                // return View("FormularioConductor", entidad);
                 return JavaScript("redireccionar('" + Url.Action("EComprobante", "Comprobante", new { idcomprobante = Entidad.IdComprobante } ) + "');");

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

        [HttpPost]
        public ActionResult Anular(long idcomprobante)
        {

            try
            {
             

                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];




                Comprobante Entidad = logica.AnularComprobante(idcomprobante);

                // return View("FormularioConductor", entidad);
                return JavaScript("redireccionar('" + Url.Action("EComprobante", "Comprobante", new { idcomprobante = Entidad.IdComprobante }) + "');");

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


        [HttpPost]
        public ActionResult Modificar(long idcomprobante, string fecha, int tipocomprobante, double cambio, long moneda, string glosa)
        {

            try
            {
                DateTime FechaComprobante;
                try
                {
                    FechaComprobante = DateTime.Parse(fecha);

                }
                catch (Exception ex)
                {
                    throw new MensageException("Ingrese la fecha con formato valido");
                }



                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];

                long nroserie = logica.obtenerSerie(empresa.idEmpresa);

                Comprobante Entidad = new Comprobante()
                {
                    IdComprobante = idcomprobante,
                    Serie = nroserie,
                    Glosa = glosa,
                    Fecha = FechaComprobante,
                    TipoCambio = cambio,
                    Estado = (int)EstadoComprobante.Abierto,
                    TipoComprobante = tipocomprobante,
                    IdEmpresa = empresa.idEmpresa,
                    IdMoneda = moneda,
                    IdUsuario = usuario.idUsuario

                };

                List<EDetalleComprobante> detalle = Session["DetalleComprobante"] as List<EDetalleComprobante>;
                EDetalleTotal total = Session["DetalleTotal"] as EDetalleTotal;
                logica.Modificar(Entidad, detalle, total);

                return JavaScript("redireccionar('" + Url.Action("Index", "Comprobante") + "');");
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


        public ActionResult ReporteComprobante(long idcomprobante)
        {
            try
            {


                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];

                List<ERDatosBasicoCuenta> datosBasico = new List<ERDatosBasicoCuenta>();
                datosBasico = logica.ReporteDatosBasicoComprobante(usuario.Usuario1, empresa.Nombre);


                List<EComprobante> comprobante = new List<EComprobante>();
                comprobante = logica.ObtenerComprobanteReporte(empresa.idEmpresa, idcomprobante);

                List<EDetalleComprobante> detalleComprobantes = new List<EDetalleComprobante>();
                detalleComprobantes = logica.listarDetalleComprobanteXComprobante(idcomprobante, empresa.idEmpresa);

                List<EDetalleTotal> eDetalleTotal = new List<EDetalleTotal>();
                eDetalleTotal = logica.TotalDetalle(detalleComprobantes);
                


                ReportViewer viewer = new ReportViewer();
                viewer.AsyncRendering = false;
                viewer.SizeToReportContent = true;

                ReportDataSource rb = new ReportDataSource("DSReporteBasicoComprobante", datosBasico);
                ReportDataSource rp = new ReportDataSource("DSReporteComprobante", comprobante);
                ReportDataSource rcdetalle = new ReportDataSource("DSReporteDetalleComprobante", detalleComprobantes);
                ReportDataSource rcdetalletotal = new ReportDataSource("DSTotales", eDetalleTotal);

                viewer.LocalReport.ReportPath = Server.MapPath("~/Reportes/ReporteComprobante.rdlc");
                viewer.LocalReport.DataSources.Clear();
                viewer.LocalReport.DataSources.Add(rp);
                viewer.LocalReport.DataSources.Add(rb);
                viewer.LocalReport.DataSources.Add(rcdetalle);
                viewer.LocalReport.DataSources.Add(rcdetalletotal);

                viewer.LocalReport.Refresh();

                ViewBag.ReporteComprobante = viewer;


                return View("ReporteComprobante");
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
        public ActionResult ReporteLibroDiario()
        {
            try
            {


                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];

                ViewBag.EmpresaMonedas = LMoneda.Logica.LMoneda.listarMonedaActivaXEmpresa(empresa.idEmpresa);
                ViewBag.Gestion = LGestion.Logica.LGestion.listarGestion(empresa.idEmpresa,usuario.idUsuario);

                return View();
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

        public ActionResult ReporteDeLibroDiario(long idgestion, long idperiodo, long idmoneda)
        {
            try
            {


                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];

                List<ERDatosBasicoComprobante> datosBasico = new List<ERDatosBasicoComprobante>();
                datosBasico = logica.ReporteDatosBasico(usuario.Usuario1, empresa.Nombre, idmoneda,idgestion,idperiodo);



                List<ELibroDiario> detalleComprobantes = new List<ELibroDiario>();
                detalleComprobantes = logica.reporteLibroDiario(idgestion,idperiodo,empresa.idEmpresa,idmoneda);

                List<EDetalleTotal> eDetalleTotal = new List<EDetalleTotal>();
                eDetalleTotal = logica.TotalLibroDiario(detalleComprobantes);



                ReportViewer viewer = new ReportViewer();
                viewer.AsyncRendering = false;
                viewer.SizeToReportContent = true;
                
                ReportDataSource rb = new ReportDataSource("DSReporteBasico", datosBasico);
                ReportDataSource rcdetalle = new ReportDataSource("DSReporteLibroDiario", detalleComprobantes);
                ReportDataSource rcdetalletotal = new ReportDataSource("DSTotalLibroDiario", eDetalleTotal);

                viewer.LocalReport.ReportPath = Server.MapPath("~/Reportes/ReporteLibroDiario.rdlc");
                viewer.LocalReport.DataSources.Clear();
               
                viewer.LocalReport.DataSources.Add(rb);
                viewer.LocalReport.DataSources.Add(rcdetalle);
                viewer.LocalReport.DataSources.Add(rcdetalletotal);

                viewer.LocalReport.Refresh();

                ViewBag.ReporteLibroDiario = viewer;


                return PartialView("LibroDiarioParcial");
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

        public ActionResult ReporteSumasSaldos()
        {
            try
            {


                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];

                ViewBag.EmpresaMonedas = LMoneda.Logica.LMoneda.listarMonedaActivaXEmpresa(empresa.idEmpresa);
                ViewBag.Gestion = LGestion.Logica.LGestion.listarGestion(empresa.idEmpresa, usuario.idUsuario);

                return View();
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


        public ActionResult ReporteDeSumasSaldos(long idgestion, long idmoneda)
        {
            try
            {


                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];

                List<ERDatosBasicoComprobante> datosBasico = new List<ERDatosBasicoComprobante>();
                datosBasico = logica.ReporteDatosBasico(usuario.Usuario1, empresa.Nombre, idmoneda, idgestion, 0);



                List<ESumasSaldos> detalleComprobantes = new List<ESumasSaldos>();
                detalleComprobantes = logica.reporteSumasSaldos(idgestion, empresa.idEmpresa, idmoneda);

                List<EDetalleTotalSumasSaldos> eDetalleTotal = new List<EDetalleTotalSumasSaldos>();
                eDetalleTotal = logica.TotalSumasSaldos(detalleComprobantes);



                ReportViewer viewer = new ReportViewer();
                viewer.AsyncRendering = false;
                viewer.SizeToReportContent = true;

                ReportDataSource rb = new ReportDataSource("DSReporteBasico", datosBasico);
                ReportDataSource rcdetalle = new ReportDataSource("DSReporteLibroDiario", detalleComprobantes);
                ReportDataSource rcdetalletotal = new ReportDataSource("DSTotalLibroDiario", eDetalleTotal);

                viewer.LocalReport.ReportPath = Server.MapPath("~/Reportes/ReporteSumasSaldos.rdlc");
                viewer.LocalReport.DataSources.Clear();

                viewer.LocalReport.DataSources.Add(rb);
                viewer.LocalReport.DataSources.Add(rcdetalle);
                viewer.LocalReport.DataSources.Add(rcdetalletotal);

                viewer.LocalReport.Refresh();

                ViewBag.ReporteSaldosSumas = viewer;


                return PartialView("SumasSaldosParcial");
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

        public ActionResult ListarPeriodo(long idgestion)
        {

        
            try
            {


                return PartialView("ListarPeriodo", LPeriodo.Logica.LPeriodo.listarPeriodo(idgestion));

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


        public ActionResult ReporteLibroMayor()
        {
            try
            {


                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];

                ViewBag.EmpresaMonedas = LMoneda.Logica.LMoneda.listarMonedaActivaXEmpresa(empresa.idEmpresa);
                ViewBag.Gestion = LGestion.Logica.LGestion.listarGestion(empresa.idEmpresa, usuario.idUsuario);

                return View();
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

        



        long ParametroIdgestion = 0;
        long ParametroIdPeriodo = 0;
        long ParametroIdmoneda = 0;
        long ParametroIdCuenta = 0;

         public ActionResult ReporteDeLibroMayor(long idgestion, long idperiodo, long idmoneda)
          {
              try
              {
                  ParametroIdgestion = idgestion;
                  ParametroIdmoneda = idmoneda;
                  ParametroIdPeriodo = idperiodo;

                  Usuario usuario = (Usuario)Session["Usuario"];
                  Empresa empresa = (Empresa)Session["Empresa"];

                  List<ERDatosBasicoComprobante> datosBasico = new List<ERDatosBasicoComprobante>();
                  datosBasico = logica.ReporteDatosBasico(usuario.Usuario1, empresa.Nombre, idmoneda, idgestion, idperiodo);

                 List<ELibroMayor> libroMayores = new List<ELibroMayor>();

                List<ELibroMayor> libroMayors = new List<ELibroMayor>();

                List<ELibroMayoCabezera> detalleComprobantes = new List<ELibroMayoCabezera>();
                detalleComprobantes = logica.reporteLibroMayorCabezera(idgestion, idperiodo, empresa.idEmpresa);

                   foreach (var i in detalleComprobantes)
                   {
                            libroMayors = logica.reporteLibroMayor(i.IdCuenta, idgestion, idperiodo, empresa.idEmpresa, idmoneda);
                        libroMayores.AddRange(libroMayors);
                    }

               
                

                  ReportViewer viewer = new ReportViewer();
                  viewer.AsyncRendering = false;
                  viewer.SizeToReportContent = true;

                  ReportDataSource rb = new ReportDataSource("DSReporteBasico", datosBasico);
                  ReportDataSource rcdetalle = new ReportDataSource("DSCabezeraLibro", libroMayores);

                  viewer.LocalReport.ReportPath = Server.MapPath("~/Reportes/ReporteLibroMayor.rdlc");
                  viewer.LocalReport.DataSources.Clear();

                  viewer.LocalReport.DataSources.Add(rb);
                  viewer.LocalReport.DataSources.Add(rcdetalle);


                  //viewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SubReporteLibroMayor);

                 // foreach(var i in detalleComprobantes)
                //  {
                 //   ParametroIdCuenta = i.IdCuenta;
                    //viewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SubReporteLibroMayor);
                 // }





                  viewer.LocalReport.Refresh();

                  ViewBag.ReporteLibroMayor = viewer;


                  return PartialView("LibroMayorParcial");
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


        public ActionResult ReporteEstadoResultado()
        {
            try
            {


                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];

                ViewBag.EmpresaMonedas = LMoneda.Logica.LMoneda.listarMonedaActivaXEmpresa(empresa.idEmpresa);
                ViewBag.Gestion = LGestion.Logica.LGestion.listarGestion(empresa.idEmpresa, usuario.idUsuario);

                return View();
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

        public ActionResult ReporteDeEstadoResultado(long idgestion, long idmoneda)
        {
            try
            {


                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];

                List<ERDatosBasicoComprobante> datosBasico = new List<ERDatosBasicoComprobante>();
                datosBasico = logica.ReporteDatosBasico(usuario.Usuario1, empresa.Nombre, idmoneda, idgestion, 0);

                /*  List<ELibroMayor> libroMayores = new List<ELibroMayor>();

                  List<ELibroMayor> libroMayors = new List<ELibroMayor>();*/


                List<EEstadoResultado> estadoresulatdo = new List<EEstadoResultado>();
                List<EEstadoResultado> auxiliar = new List<EEstadoResultado>();
                List<EEstadoResultado> ingresos = new List<EEstadoResultado>();
                List<EEstadoResultado> egresos = new List<EEstadoResultado>();
                ingresos = logica.CabezaraEstadoResultado(idgestion, empresa.idEmpresa, (int)TipoEstadoResultado.Ingreso);

                foreach (var i in ingresos)
                {
                    auxiliar = logica.EstadoResultado(i.IdCuenta, idgestion, empresa.idEmpresa, idmoneda, (int)TipoEstadoResultado.Ingreso);
                    estadoresulatdo.AddRange(auxiliar);
                }

                double Utilidad = 0;

                EEstadoResultado total = new EEstadoResultado();
                total.Cuenta = "TOTAL INGRESOS";
                total.IdCuenta = 0;
                total.TipoComprobante = "Ingreso";
                total.Total = 0;

                foreach (var i in estadoresulatdo)
                {
                    if (i.TipoComprobante.Equals("Ingreso"))
                    {
                        total.Total = Math.Round((total.Total + i.Total), 2);
                    }
                
                }

                estadoresulatdo.Add(total);

                Utilidad = total.Total;


                //Egresos

                egresos = logica.CabezaraEstadoResultado(idgestion, empresa.idEmpresa, (int)TipoEstadoResultado.Egreso);

                foreach (var i in egresos)
                {
                    auxiliar = logica.EstadoResultado(i.IdCuenta, idgestion, empresa.idEmpresa, idmoneda, (int)TipoComprobante.Egreso);
                    estadoresulatdo.AddRange(auxiliar);
                }

         

                EEstadoResultado totale = new EEstadoResultado();
                totale.Cuenta = "TOTAL EGRESOS";
                totale.IdCuenta = 0;
                totale.TipoComprobante = "Egreso";
                totale.Total = 0;

                foreach (var i in estadoresulatdo)
                {
                    if (i.TipoComprobante.Equals("Egreso"))
                    {
                        totale.Total = Math.Round((totale.Total + i.Total), 2);
                    }
                 
                }

                estadoresulatdo.Add(totale);

                Utilidad =  Math.Round((Utilidad - totale.Total), 2);


                List<ETotalEstado> totalEstados = new List<ETotalEstado>();
                totalEstados = logica.totalEstados(Utilidad);


                ReportViewer viewer = new ReportViewer();
                viewer.AsyncRendering = false;
                viewer.SizeToReportContent = true;

                ReportDataSource rb = new ReportDataSource("DSReporteBasico", datosBasico);
                ReportDataSource rcdetalle = new ReportDataSource("DSReporteEstado", estadoresulatdo);
                ReportDataSource rctotal = new ReportDataSource("DSTotal", totalEstados);

                viewer.LocalReport.ReportPath = Server.MapPath("~/Reportes/ReporteEstadoResultado.rdlc");
                viewer.LocalReport.DataSources.Clear();

                viewer.LocalReport.DataSources.Add(rb);
                viewer.LocalReport.DataSources.Add(rcdetalle);
                viewer.LocalReport.DataSources.Add(rctotal);






                viewer.LocalReport.Refresh();

                ViewBag.ReporteEstadoR = viewer;


                return PartialView("LibroEstadoResultadoParcial");
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

        /*  public ActionResult ReporteDeLibroMayor(long idgestion, long idperiodo, long idmoneda)
         {
             try
             {
                 ParametroIdgestion = idgestion;
                 ParametroIdmoneda = idmoneda;
                 ParametroIdPeriodo = idperiodo;

                 Usuario usuario = (Usuario)Session["Usuario"];
                 Empresa empresa = (Empresa)Session["Empresa"];

                 List<ERDatosBasicoComprobante> datosBasico = new List<ERDatosBasicoComprobante>();
                 datosBasico = logica.ReporteDatosBasico(usuario.Usuario1, empresa.Nombre, idmoneda, idgestion, idperiodo);



                 List<ELibroMayoCabezera> detalleComprobantes = new List<ELibroMayoCabezera>();
                 detalleComprobantes = logica.reporteLibroMayorCabezera(idgestion, idperiodo, empresa.idEmpresa);







                 ReportViewer viewer = new ReportViewer();
                 viewer.AsyncRendering = false;
                 viewer.SizeToReportContent = true;

                  // ReportDataSource rb = new ReportDataSource("DSReporteBasico", datosBasico);
                  // ReportDataSource rcdetalle = new ReportDataSource("DSCabezeraLibro", detalleComprobantes);

                  List<ELibroMayor> libroMayors = new List<ELibroMayor>();
                  libroMayors = logica.reporteLibroMayor(detalleComprobantes.FirstOrDefault().IdCuenta, idgestion, idperiodo, empresa.idEmpresa, idmoneda);

                  ReportDataSource ds = new ReportDataSource("DSSubReporteLibroMayor", libroMayors);

                  viewer.LocalReport.ReportPath = Server.MapPath("~/Reportes/SubReporteLibroMayor.rdlc");
                 viewer.LocalReport.DataSources.Clear();

                 viewer.LocalReport.DataSources.Add(ds);
                 //viewer.LocalReport.DataSources.Add(rcdetalle);
               //  viewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SubReporteLibroMayor);


                 viewer.LocalReport.Refresh();

                 ViewBag.ReporteLibroMayor = viewer;


                 return PartialView("LibroMayorParcial");
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

         }*/


        void SubReporteLibroMayor(object sender, SubreportProcessingEventArgs e)
        {
            try
            {

                // long IdCuenta = long.Parse(e.Parameters["IdCuenta"].Values[0].ToString());
                long IdCuenta = ParametroIdCuenta;
                long idperiodo = ParametroIdPeriodo;
                long idmoneda = ParametroIdmoneda;
                long idgestion = ParametroIdgestion;

                Empresa empresa = (Empresa)Session["Empresa"];

                List<ELibroMayor> libroMayors = new List<ELibroMayor>();
                libroMayors = logica.reporteLibroMayor(10139, idgestion, idperiodo, empresa.idEmpresa, idmoneda);

                ReportDataSource ds = new ReportDataSource("DSSubReporteLibroMayor", libroMayors);
                e.DataSources.Add(ds);
            }
            catch(Exception ex)
            {
                // return ex.Message;
                string a = ex.Message;
            }
          

        }



    }
}
