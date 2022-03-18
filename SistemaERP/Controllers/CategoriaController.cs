using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Logica;
using Entidad;
using Datos;

namespace SistemaERP.Controllers
{
    public class CategoriaController : Controller
    {
        private LCategoria logica = LCategoria.Logica.LCategoria;
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ListarCategoria()
        {


            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];

                List<ECategoria> categorias = new List<ECategoria>();
                categorias = logica.listarCategorias(empresa.idEmpresa);

               

                return Json(new
                {
                    Categorias = categorias

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
        public ActionResult ObtenerCategoria(long idcategoria)
        {

      

            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];

             
                var entidad = logica.obtenerCategoria(idcategoria);


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
        public ActionResult Registro(string nombre, long idcategoria, long idPadre)
        {

            try
            {





                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];



                if (idPadre == 0)
                {
                    //hijo 0
                    Categoria categoria = new Categoria()
                    {
                        Nombre = nombre,
                        IdUsuario = usuario.idUsuario,
                        IdEmpresa = empresa.idEmpresa,
                        IdCategoriaPadre = idcategoria


                    };
                    logica.Registro(categoria, idcategoria, 0);
                }

                else if (idPadre == 1)
                {
                    //padre 1
                    Categoria categoria = new Categoria()
                    {
                        Nombre = nombre,
                        IdUsuario = usuario.idUsuario,
                        IdEmpresa = empresa.idEmpresa,


                    };
                    logica.Registro(categoria, 0, 1);
                }


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
        public ActionResult Editar(string nombre, long idCategoria)
        {

            try
            {





                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];



                logica.ModificarCategoria(idCategoria, nombre, empresa.idEmpresa);

               
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
        public ActionResult Eliminar(long idcategoria)
        {

            try
            {





                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa empresa = (Empresa)Session["Empresa"];



                logica.eliminarCategoria(idcategoria);

           
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



    }
}
