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
    public class CuentaController : Controller
    {

        private LCuenta logica = LCuenta.Logica.LCuenta;
        public ActionResult Index()
        {


            return View();
        }

        [HttpPost]
        public ActionResult ListarCuenta()
        {

            /* Usuario usuario = (Usuario)Session["Usuario"];
             Empresa empresa = (Empresa)Session["Empresa"];



             return View();*/

            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];

                List<ECuenta> cuentas = new List<ECuenta>();
                cuentas = logica.listarCuentas(usuario.idUsuario, empresa.idEmpresa);

                /*
                 *Registrar hijos
                 * 
                 * Cuenta cuenta = new Cuenta()
                 {
                     Nombre = "prueba registro 2",
                     Codigo = "",
                     Nivel = 0,
                     TipoCuenta = "detalle",
                     IdUsuario = usuario.idUsuario,
                     IdEmpresa = empresa.idEmpresa,
                     IdCuentaPadre = 1


                 };

                 logica.Registro(cuenta, 1, 0);*/

                /*    Cuenta cuenta = new Cuenta()
                    {
                        Nombre = "prueba registro 3",
                        Codigo = "",
                        Nivel = 0,
                        TipoCuenta = "detalle",
                        IdUsuario = usuario.idUsuario,
                        IdEmpresa = empresa.idEmpresa,
                        IdCuentaPadre = 3


                    };
                    // 0 hijo
                    // 1 padre
                    logica.Registro(cuenta, 3, 0);*/

                return Json(new
                {
                    Cuentas = cuentas

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
        public ActionResult ObtenerCuenta(long idcuenta)
        {

            /* Usuario usuario = (Usuario)Session["Usuario"];
             Empresa empresa = (Empresa)Session["Empresa"];



             return View();*/

            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];

                //List<ECuenta> cuentas = new List<ECuenta>();
                var entidad = logica.obtenerCuenta(idcuenta);


                return Json(new
                {
                    Nombre = entidad.Nombre

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
        public ActionResult Registro(string nombre, long idCuenta, long idPadre)
        {

            try
            {





                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];



                if (idPadre == 0)
                {
                    //hijo 0
                    Cuenta cuenta = new Cuenta()
                    {
                        Nombre = nombre,
                        Codigo = "",
                        Nivel = 0,
                        TipoDeCuenta = (int)TipoCuenta.Global,
                        IdUsuario = usuario.idUsuario,
                        IdEmpresa = empresa.idEmpresa,
                        IdCuentaPadre = idCuenta


                    };
                    logica.Registro(cuenta, idCuenta, 0);
                }

                else if (idPadre == 1)
                {
                    //padre 1
                    Cuenta cuenta = new Cuenta()
                    {
                        Nombre = nombre,
                        Codigo = "",
                        Nivel = 0,
                        TipoDeCuenta = (int)TipoCuenta.Global,
                        IdUsuario = usuario.idUsuario,
                        IdEmpresa = empresa.idEmpresa,



                    };
                    logica.Registro(cuenta, 0, 1);
                }


                // 0 hijo
                // 1 padre


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
        public ActionResult Editar(string nombre, long idCuenta)
        {

            try
            {





                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];



                logica.ModificarCuenta(idCuenta, nombre, empresa.idEmpresa);

                // 0 hijo
                // 1 padre


                //List<Empresa> empresas = new List<Empresa>();
                // empresas = logica.listarEmpresa(usuario.idUsuario);
                // return PartialView("_ListaEmpresa", empresas);
                return JavaScript("MostrarMensajeExitoEditar('Modificacion Exitoso');");
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
        public ActionResult Eliminar(long idCuenta)
        {

            try
            {





                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];



                logica.EliminarCuenta(idCuenta);

                // 0 hijo
                // 1 padre


                //List<Empresa> empresas = new List<Empresa>();
                // empresas = logica.listarEmpresa(usuario.idUsuario);
                // return PartialView("_ListaEmpresa", empresas);
                return JavaScript("MostrarMensajeEliminacion('Eliminación Exitosa');");
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



        public ActionResult ReporteCuenta()
        {
            try
            {


                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];
              //  Gestion gestion = (Gestion)Session["Gestion"];
                List<ECuenta> cuentas = new List<ECuenta>();
                cuentas = logica.listarCuentasReporte(usuario.idUsuario, empresa.idEmpresa);

                List<ERDatosBasicoCuenta> datosBasico = new List<ERDatosBasicoCuenta>();
                datosBasico = logica.ReporteDatosBasicoCuenta(usuario.Usuario1, empresa.Nombre);
                ReportViewer viewer = new ReportViewer();
                viewer.AsyncRendering = false;
                viewer.SizeToReportContent = true;

                ReportDataSource rp = new ReportDataSource("DSReporteCuenta", cuentas);
                ReportDataSource rb = new ReportDataSource("DSReporteBasicoCuenta", datosBasico);
                viewer.LocalReport.ReportPath = Server.MapPath("~/Reportes/ReporteCuenta.rdlc");
                viewer.LocalReport.DataSources.Clear();
                viewer.LocalReport.DataSources.Add(rp);
                viewer.LocalReport.DataSources.Add(rb);

                viewer.LocalReport.Refresh();

                ViewBag.ReporteCuenta = viewer;


                return View("ReporteCuenta");
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

        public ActionResult CuentaIntegracion()
        {


            Usuario usuario = (Usuario)Session["Usuario"];
            Empresa empresa = (Empresa)Session["Empresa"];

            ViewBag.CuentasDetalle = logica.listarCuentaDetalle(empresa.idEmpresa);



            return View(LEmpresa.Logica.LEmpresa.obtenerEmpresa(empresa.idEmpresa));
        }

        [HttpPost]
        public ActionResult RegistroCuentaIntegracion(int integracion,long IdCaja, long IdCreditoFiscal, long IdDebitoFiscal, long IdCompras, long IdVentas,long IdIt, long IdItPagar )
        {

            try
            {



                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];

                Empresa Entidad = new Empresa()
                {
                    idEmpresa = empresa.idEmpresa,
                    Integracion = integracion,
                    IdCuentaCaja = IdCaja,
                    IdCuentaCompras = IdCompras,
                    IdCuentaCreditoFiscal = IdCreditoFiscal,
                    IdCuentaDebitoFiscal = IdDebitoFiscal,
                    IdCuentaIt = IdIt,
                    IdCuentaItPorPagar = IdItPagar,
                    IdCuentaVentas = IdVentas
                    
                };

                logica.RegistroIntegraciones(Entidad);

                // 0 hijo
                // 1 padre


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
    }
}
