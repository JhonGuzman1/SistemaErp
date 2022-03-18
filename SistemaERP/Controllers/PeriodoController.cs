using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Logica;
using Datos;
using Entidad;
using Entidad.Estados;
using Microsoft.Reporting.WebForms;

namespace SistemaERP.Controllers
{
    public class PeriodoController : Controller
    {
        private LPeriodo logica = LPeriodo.Logica.LPeriodo;
        public ActionResult Index()
        {

            try
            {
                //Usuario usuario = (Usuario)Session["Usuario"];
                //Empresa empresa = (Empresa)Session["Empresa"];
                Gestion gestion = (Gestion)Session["Gestion"];
                List<Periodo> periodos = new List<Periodo>();
                periodos = logica.listarPeriodo(gestion.IdGestion);

                return View(periodos);
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
                Gestion gestion = (Gestion)Session["Gestion"];
                List<Periodo> periodos = new List<Periodo>();
                periodos = logica.listarPeriodo(gestion.IdGestion);
                return PartialView("ListaPeriodo", periodos);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(400, ex.Message.Replace("'", ""));
            }


        }

        [HttpPost]
        public ActionResult Registro(string nombre, string fechainicio, string fechafin)
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
                Gestion gestion = (Gestion)Session["Gestion"];
                // Empresa empresa = (Empresa)Session["Empresa"];
                // Entidades = lLogica.ObtenerLista(estado);
                Periodo periodo = new Periodo()
                {
                    Nombre = nombre,
                    FechaInicio = DateTime.Parse(fechainicio),
                    FechaFin = DateTime.Parse(fechafin),
                    Estado = (int)EstadoPeriodo.Abierto,
                    IdGestion = gestion.IdGestion,
                    IdUsuario = usuario.idUsuario

                };
                logica.Registro(periodo);
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
                Gestion gestion = (Gestion)Session["Gestion"];
              
                Periodo periodo = new Periodo()
                {
                    idPeriodo = id,
                    Nombre = nombre,
                    FechaInicio = DateTime.Parse(fechainicio),
                    FechaFin = DateTime.Parse(fechafin),
                    Estado = (int)EstadoPeriodo.Abierto,
                    IdGestion = gestion.IdGestion,
                    IdUsuario = usuario.idUsuario

                };
                logica.Editar(periodo);
              
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

        public ActionResult ReportePeriodo()
        {
            try
            {


                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];
                Gestion gestion = (Gestion)Session["Gestion"];
                List<ERPeriodo> periodos = new List<ERPeriodo>();
                periodos = logica.ReportePeriodo(gestion.IdGestion);

                List<ERDatosBasicoPeriodo> datosBasico = new List<ERDatosBasicoPeriodo>();
                datosBasico = logica.ReporteDatosBasicoPeriodo(usuario.Usuario1, empresa.Nombre,gestion.Nombre);
                ReportViewer viewer = new ReportViewer();
                viewer.AsyncRendering = false;
                viewer.SizeToReportContent = true;
                viewer.Width = 400;
                viewer.Height = 800;

                ReportDataSource rp = new ReportDataSource("DSReportePeriodo", periodos);
                ReportDataSource rb = new ReportDataSource("DSReporteBasicoPeriodo", datosBasico);
                viewer.LocalReport.ReportPath = Server.MapPath("~/Reportes/ReportePeriodo.rdlc");
                viewer.LocalReport.DataSources.Clear();
                viewer.LocalReport.DataSources.Add(rp);
                viewer.LocalReport.DataSources.Add(rb);

                viewer.LocalReport.Refresh();

                ViewBag.ReporteGestion = viewer;


                return View("ReportePeriodo");
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
