using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entidad;
using Datos;
using Logica;
using Entidad.Estados;

namespace SistemaERP.Controllers
{
    public class MonedaController : Controller
    {

        private LMoneda logica = LMoneda.Logica.LMoneda;
        public ActionResult Index()
        {
            Empresa empresa = (Empresa)Session["Empresa"];

            ViewBag.MonedaPrincipal = LMoneda.Logica.LMoneda.listarMonedaPrincipal(empresa.idEmpresa);
            ViewBag.MonedasAlternativa = LMoneda.Logica.LMoneda.listarMonedaAlterntaiva(empresa.idEmpresa);
           // List<EmpresaMoneda> monedas = logica.listarMonedaXEmpresa(empresa.idEmpresa);
            return View(logica.listarMonedaXEmpresa(empresa.idEmpresa));
        }

        [HttpPost]
        public ActionResult ObtenerMoneda()
        {
            try
            {


                Empresa empresa = (Empresa)Session["Empresa"];
                var moneda = LMoneda.Logica.LMoneda.obtenerMonedadXempresa(empresa.idEmpresa);


                return Json(new
                {

                    idMonedaPadre = moneda.IdMonedaPrincipal,
                    idMonedaAlternativa = moneda.IdMonedaAlternativa,
                    Cambio = (moneda.Cambio != null) ? moneda.Cambio.ToString().Replace(",",".") : ""

                }); ;

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


        public ActionResult Listar()
        {

            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];
               
                return PartialView("ListaMoneda", logica.listarMonedaXEmpresa(empresa.idEmpresa));
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(400, ex.Message.Replace("'", ""));
            }


        }



        [HttpPost]
        public ActionResult Registro(long idMonedaPrincipal, long idMonedaAlternativa, double? cambio)
        {

            try
            {

              


                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];
                // Entidades = lLogica.ObtenerLista(estado);
                EmpresaMoneda empresaMoneda = new EmpresaMoneda()
                {
                    Activo = (int)EstadoMonedaEmpresa.Activo,
                    FechaRegistro = DateTime.Now,
                    IdEmpresa = empresa.idEmpresa,
                    IdMonedaPrincipal = idMonedaPrincipal,
                    Cambio = cambio,
                    IdMonedaAlternativa = idMonedaAlternativa,
                    IdUsuario = usuario.idUsuario

                };
                logica.Registro(empresaMoneda);
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
