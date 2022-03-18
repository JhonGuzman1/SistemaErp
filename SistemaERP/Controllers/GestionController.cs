using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Logica;
using Datos;
using Entidad.Estados;
using Entidad;
using Microsoft.Reporting.WebForms;

namespace SistemaERP.Controllers
{
    public class GestionController : Controller
    {

        private LGestion logica = LGestion.Logica.LGestion;
        public ActionResult Index()
        {

            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];
                List<Gestion> gestion = new List<Gestion>();
                gestion = logica.listarGestion(empresa.idEmpresa,usuario.idUsuario);

                return View(gestion);
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('Ha ocurrido un error.');");
            }
        }


        public ActionResult Listar()
        {

            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];
                List<Gestion> gestion = new List<Gestion>();
                gestion = logica.listarGestion(empresa.idEmpresa, usuario.idUsuario);
                return PartialView("ListaGestion", gestion);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(400, ex.Message.Replace("'", ""));
            }


        }


        [HttpPost]
        public ActionResult Registro(string nombre,string fechainicio,string fechafin)
        {

            try
            {

                try
                {
                    DateTime FechaInicio = DateTime.Parse(fechainicio);
                    DateTime FechaFin = DateTime.Parse(fechafin);
                }
                catch(Exception ex)
                {
                    throw new MensageException("Ingrese las fechas con formato valido");
                }
                


                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];
                // Entidades = lLogica.ObtenerLista(estado);
                Gestion gestion = new Gestion()
                {
                    Nombre = nombre,
                    FechaInicio = DateTime.Parse(fechainicio),
                    FechaFin = DateTime.Parse(fechafin),
                    Estado = (int)EstadoGestion.Abierto,
                    IdEmpresa = empresa.idEmpresa,
                    IdUsuario = usuario.idUsuario

                };
                logica.Registro(gestion);
                //List<Empresa> empresas = new List<Empresa>();
                // empresas = logica.listarEmpresa(usuario.idUsuario);
                // return PartialView("_ListaEmpresa", empresas);
                return JavaScript("MostrarMensajeExito('Registro Exitoso');");
            }
            catch (MensageException ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message +"');");
            }

        }


       // [HttpPost]
        public ActionResult ValidarGestionAbierta()
        {

            try
            {

                Empresa empresa = (Empresa)Session["Empresa"];
                logica.ValidarGestionesAbiertas(empresa.idEmpresa);
                //List<Empresa> empresas = new List<Empresa>();
                // empresas = logica.listarEmpresa(usuario.idUsuario);
                // return PartialView("_ListaEmpresa", empresas);
                return JavaScript("AbrirModalRegistro();");
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
        public ActionResult Editar(long id, string nombre, string fechainicio, string fechafin)
        {

            try
            {

                try
                {
                    DateTime FechaInicio = DateTime.Parse(fechainicio);
                    DateTime FechaFin = DateTime.Parse(fechafin);
                }
                catch (Exception ex)
                {
                    throw new MensageException("Ingrese las fechas con formato valido");
                }



                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];
                // Entidades = lLogica.ObtenerLista(estado);
                Gestion gestion = new Gestion()
                {
                    IdGestion = id,
                    Nombre = nombre,
                    FechaInicio = DateTime.Parse(fechainicio),
                    FechaFin = DateTime.Parse(fechafin),
                    Estado = (int)EstadoGestion.Abierto,
                    IdEmpresa = empresa.idEmpresa,
                    IdUsuario = usuario.idUsuario

                };
                logica.Editar(gestion);
                //List<Empresa> empresas = new List<Empresa>();
                // empresas = logica.listarEmpresa(usuario.idUsuario);
                // return PartialView("_ListaEmpresa", empresas);
                return JavaScript("MostrarMensajeExito('Modificación Exitosa');");
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
        public ActionResult Eliminar(long id)
        {
            try
            {
                logica.Eliminar(id);
                // string mensaje = "Registro Exitoso";

                return JavaScript("MostrarMensajeEliminacion('Eliminación Exitosa');");

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


        [HttpPost]
        public JavaScriptResult IngresarGestion(long id)
        {
            try
            {
                
                Gestion gestion = logica.obtenerGestion(id);
                Session["Gestion"] = gestion;
                return JavaScript("redireccionar('" + Url.Action("Index", "Periodo") + "');");
                // }
            }
            catch (Exception ex)
            {
                string mensaje = ex.Message.Replace("'", "");
                //ViewBag.Mensaje = mensaje;
                return JavaScript("MostrarMensaje('" + mensaje + "');");
            }
        }


        public ActionResult ReporteGestion()
        {
            try
            {


                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];
                List<ERGestion> gestiones = new List<ERGestion>();
                gestiones = logica.ReporteGestion(empresa.idEmpresa);

                List<ERDatosBasicoGestion> datosBasico = new List<ERDatosBasicoGestion>();
                datosBasico = logica.ReporteDatosBasicoGestion(usuario.Usuario1,empresa.Nombre);
                ReportViewer viewer = new ReportViewer();
                viewer.AsyncRendering = false;
                viewer.SizeToReportContent = true;

                ReportDataSource rp = new ReportDataSource("DSReporteGestion", gestiones);
                ReportDataSource rb = new ReportDataSource("DSReporteBasicoGestion", datosBasico);
                viewer.LocalReport.ReportPath = Server.MapPath("~/Reportes/ReporteGestion.rdlc");
                viewer.LocalReport.DataSources.Clear();
                viewer.LocalReport.DataSources.Add(rp);
                viewer.LocalReport.DataSources.Add(rb);
           
                viewer.LocalReport.Refresh();

                ViewBag.ReporteGestion = viewer;

             
                return View("ReporteGestion");
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
