using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entidad;
using Datos;
using Logica;

namespace SistemaERP.Controllers
{
    public class ArticuloController : Controller
    {

        private LArticulo logica = LArticulo.Logica.LArticulo;
        // GET: Articulo
        public ActionResult Index()
        {

            try
            {

                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];

                ViewBag.Categorias = LCategoria.Logica.LCategoria.listadosdecategorias(empresa.idEmpresa);

                return View(logica.listarArticulo(empresa.idEmpresa));
            }
            catch(Exception ex)
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
               
                return PartialView("ListaArticulo", logica.listarArticulo(empresa.idEmpresa));
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(400, ex.Message.Replace("'", ""));
            }


        }

        [HttpPost]
        public ActionResult Registro(string nombre, string descripcion, double precio, List<ECategoriaJSON> categorias)
        {

            try
            {

                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];


               


                Articulo Entidad = new Articulo()
                {
                    Nombre = nombre,
                    Descripcion = descripcion,
                    PrecioVenta = precio,
                    Cantidad = 0,
                    IdEmpresa = empresa.idEmpresa,
                    IdUsuario = empresa.IdUsuario

                };

                logica.Registro(Entidad,categorias);
             
             
               
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
        public ActionResult ObtenerArticulo(long idarticulo)
        {

          
            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];


                EArticulo articulo = logica.ObtenerArticulo(idarticulo, empresa.idEmpresa);
            
              

                return Json(new
                {
                   Data = articulo
                   
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
        public ActionResult ELiminar(long idarticulo)
        {

            try
            {

                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];





              

                logica.Eliminar(idarticulo, empresa.idEmpresa);



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


        [HttpPost]
        public ActionResult Modificar(long idarticulo, string nombre, string descripcion, double precio, List<ECategoriaJSON> categorias)
        {

            try
            {

                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];





                Articulo Entidad = new Articulo()
                {
                    IdArticulo = idarticulo,
                    Nombre = nombre,
                    Descripcion = descripcion,
                    PrecioVenta = precio,
                    // Categoria = categorias,
                    IdEmpresa = empresa.idEmpresa,
                    IdUsuario = empresa.IdUsuario

                };

                logica.Modificar(Entidad, categorias);



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


        public ActionResult ListarLote(long idarticulo)
        {


            try
            {


                return PartialView("ListaLotes", logica.ListarLotes(idarticulo));

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
    }
}
