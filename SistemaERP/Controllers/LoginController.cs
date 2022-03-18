using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Logica;
using Datos;

namespace SistemaERP.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {

        private LUsuario LUsuario = LUsuario.Logica.LUsuario;
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

       
        [HttpPost]
        public JavaScriptResult IniciarSesion(string usuario, string contrasena)
        {
            try
            {

                Usuario user = LUsuario.IniciarSesion(usuario, contrasena);
                if (user == null)
                {
                    throw new Exception("Usuario o Contraseña incorrecta");
                }
                else
                {
                    Session["Bloqueado"] = false;
                    Session["Usuario"] = user;
                    return JavaScript("redireccionar('" + Url.Action("Index", "Empresa") + "');");
                }
            }
            catch (Exception ex)
            {
                string mensaje = ex.Message.Replace("'", "");
                ViewBag.Mensaje = mensaje;
                return JavaScript("MostrarMensaje('" + mensaje + "');");
            }
        }


        public ActionResult CerrarSesion()
        {
            Session.RemoveAll();
            return RedirectToAction("Index", "Login");
        }

    }
}
