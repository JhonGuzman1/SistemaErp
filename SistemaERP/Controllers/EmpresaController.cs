using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Logica;
using Datos;
using Entidad;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;
using Entidad.Estados;

namespace SistemaERP.Controllers
{
    public class EmpresaController : Controller
    {

        private LEmpresa logica = LEmpresa.Logica.LEmpresa;
       
        // GET: Empresa
        public ActionResult Index()
        {

            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                Session["Empresa"] = null;
                Session["EmpresaSelect"] = null;
                //ViewBag.Empresa = null;
                ViewBag.Monedas = LMoneda.Logica.LMoneda.listarMoneda();
                List<Empresa> empresas = new List<Empresa>();
                empresas = logica.listarEmpresa(usuario.idUsuario);

               

                return View(empresas);
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('Ha ocurrido un error.');");
            }
        }
        /* [HttpPost]
         public JavaScriptResult ObtenerEmpresa(long id)
         {
             try
             {
                     Empresa empresa = new Empresa();
                     empresa = logica.obtenerEmpresa(id);
                     string mensaje = "Registro Exitoso";
                     return JavaScript("MostrarMensajeExito('"+empresa+"');");

             }
             catch(MensageException ex)
             {
                 string mensaje = ex.Message.Replace("'", "");
                 ViewBag.Mensaje = mensaje;
                 return JavaScript("MostrarMensaje('" + mensaje + "');");
             }
             catch (Exception ex)
             {
                 string mensaje = ex.Message.Replace("'", "");
                 ViewBag.Mensaje = mensaje;
                 return JavaScript("MostrarMensaje('" + mensaje + "');");
             }

         }*/

        [HttpPost]
        public ActionResult ObtenerEmpresa(long id)
        {
            try
            {
                Empresa empresa = new Empresa();
                empresa = logica.obtenerEmpresa(id);
                // string mensaje = "Registro Exitoso";
                //long idMoneda =
                var moneda = LMoneda.Logica.LMoneda.obtenerMonedadXempresa(empresa.idEmpresa);

                long idMoneda = -1;
                if (moneda != null)
                    idMoneda = moneda.IdMonedaPrincipal;

                return Json(new
                {
                    idEmpresa = empresa.idEmpresa,
                    Nombre = empresa.Nombre,
                    Nit = empresa.NIT,
                    Sigla= empresa.Sigla,
                    Telefono= empresa.Telefono,
                    Correo = empresa.Correo,
                    Direccion = empresa.Direccion,
                    Niveles = empresa.Niveles,
                    idMoneda= idMoneda

                }); 

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
        public ActionResult Registro(string nombre, string nit,string sigla, string telefono,string correo,string direeccion,int nivel,long idmoneda)
        {
  
            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                // Entidades = lLogica.ObtenerLista(estado);
                Empresa empresa = new Empresa()
                {
                    Nombre = nombre,
                    NIT = nit,
                    Sigla = sigla,
                    Telefono = telefono,
                    Correo = correo,
                    Direccion = direeccion,
                    Niveles = nivel,
                    Integracion = (int)Integracion.No,
                    IdUsuario = usuario.idUsuario
                    
                };

                Session["EmpresaSelect"] = logica.Registro(empresa,idmoneda);
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
                return JavaScript("MostrarMensaje('Ha ocurrido un error');");
            }

        }

        [HttpPost]
        public ActionResult Editar(long id, string nombre, string nit, string sigla, string telefono, string correo, string direeccion, int nivel,long idmoneda)
        {

            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                // Entidades = lLogica.ObtenerLista(estado);
                Empresa empresa = new Empresa()
                {
                    idEmpresa = id,
                    Nombre = nombre,
                    NIT = nit,
                    Sigla = sigla,
                    Telefono = telefono,
                    Correo = correo,
                    Direccion = direeccion,
                    Niveles = nivel,
                    IdUsuario = usuario.idUsuario

                };
                Session["EmpresaSelect"]  = logica.Editar(empresa,idmoneda);
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
                return JavaScript("MostrarMensaje('Ha ocurrido un error');");
            }

        }


        public ActionResult ListarEmpresa()
        {
          
            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                List<Empresa> empresas = new List<Empresa>();
                empresas = logica.listarEmpresa(usuario.idUsuario);
                return PartialView("_ListaEmpresa", empresas);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(400, ex.Message.Replace("'", ""));
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
        public JavaScriptResult IngresarEmpresa(long id)
        {
            try
            {
                /*
                                Usuario user = LUsuario.IniciarSesion(usuario, contrasena);
                                if (user == null)
                                {
                                    throw new Exception("Usuario o Contraseña incorrecta");
                                }
                                else
                                {
                                    Session["Bloqueado"] = false;
                                    Session["Usuario"] = user;*/
                Empresa empresa = logica.obtenerEmpresa(id);
                Session["Empresa"] = empresa;
                return JavaScript("redireccionar('" + Url.Action("Index", "Inicio") + "');");
               // }
            }
            catch (Exception ex)
            {
                string mensaje = ex.Message.Replace("'", "");
                //ViewBag.Mensaje = mensaje;
                return JavaScript("MostrarMensaje('" + mensaje + "');");
            }
        }

       // [HttpPost]
        public ActionResult ReporteEmpresas()
        {
            try
            {


                Usuario usuario = (Usuario)Session["Usuario"];
                List<EREmpresa> empresas = new List<EREmpresa>();
                empresas = LEmpresa.Logica.LEmpresa.ReporteListaEmpresaReportView();

                List<ERDatosBasico> datosBasico = new List<ERDatosBasico>();
                datosBasico = LEmpresa.Logica.LEmpresa.ReporteDatosBasico(usuario.Usuario1);
                ReportViewer viewer = new ReportViewer();
                viewer.AsyncRendering = false;
                viewer.SizeToReportContent = true;
               
              //  viewer.ZoomMode = ZoomMode.FullPage;
                // var viewer = new Microsoft.Reporting.WebForms.ReportViewer();
                ReportDataSource rp = new ReportDataSource("DSReporteEmpresas", empresas);
                ReportDataSource rb = new ReportDataSource("DSReporteBasico", datosBasico);
                viewer.LocalReport.ReportPath = Server.MapPath("~/Reportes/ReporteEmpresa.rdlc");
                viewer.LocalReport.DataSources.Clear();
                viewer.LocalReport.DataSources.Add(rp);
                viewer.LocalReport.DataSources.Add(rb);
                ///var parameters = viewer.LocalReport.GetParameters();
           
                //  viewer.SizeToReportContent = true;
                //  viewer.AsyncRendering = false;
                // viewer.Width = new Unit(Report.Width); 
                // viewer.Height = new Unit(Report.Width); 
                viewer.LocalReport.Refresh();
            
                ViewBag.Reporte = viewer;

                // logica.Eliminar(id);
                // string mensaje = "Registro Exitoso";
                /*  Usuario usuario = (Usuario)Session["Usuario"];
                  List<EREmpresa> empresas = new List<EREmpresa>();
                  empresas = logica.ReporteListaEmpresaReportView();

                  List<ERDatosBasico> datosBasico = new List<ERDatosBasico>();
                  datosBasico = logica.ReporteDatosBasico(usuario.Usuario1);*/

                /* var viewer = new Microsoft.Reporting.WebForms.ReportViewer();
                 viewer.ProcessingMode = 0;
                 viewer.LocalReport.ReportPath = Request.MapPath("~/Reportes/ReporteEmpresa.rdlc");
                 viewer.LocalReport.DataSources.Clear();
                 viewer.LocalReport.DataSources.Add(new ReportDataSource("DSReporteEmpresas", empresas));
                 viewer.LocalReport.DataSources.Add(new ReportDataSource("DSReporteBasico", datosBasico));
                 viewer.LocalReport.Refresh();*/
                // FormReporteEmpresa.CreateHtmlTextWriterFromType.Execute();

                //return JavaScript("redireccionar('" + Url.Action("FormReporteEmpresa.aspx", "Empresa") + "');");
                return View("ReporteEmpresas");
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
