using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datos;
using Entidad;
namespace Logica
{
    public class LUsuario : LLogica<Usuario>
    {


        public Usuario IniciarSesion(string User, string Password)
        {
            using (var esquema = GetEsquema())
            {
                Usuario usuario = null;
                try
                {
                    usuario = (from u in esquema.Usuario
                               where u.Usuario1 == User && u.Pass == Password 
                               select u).FirstOrDefault();
                }
                catch (Exception ex)
                {
                  throw new MensageException(ex.Message+"");
                }
                if (usuario != null)
                {
                   
                   return usuario;
                }
                else
                {
                  throw new MensageException("Usuario o Contraseña incorrecta");
                }
            }
        }


        public bool PerimisosController(string controller)
        {

            List<Permisos> permisos = new List<Permisos>();
            permisos.Add(new Permisos("Gestion"));
            permisos.Add(new Permisos("Inicio"));
            permisos.Add(new Permisos("Periodo"));
            permisos.Add(new Permisos("Cuenta"));
            permisos.Add(new Permisos("Moneda"));
            permisos.Add(new Permisos("Comprobante"));
            permisos.Add(new Permisos("Categoria"));
            permisos.Add(new Permisos("Articulo"));
            permisos.Add(new Permisos("NotaCompra"));
            permisos.Add(new Permisos("NotaVenta"));

            foreach (var i in permisos )
            {
                if (i.Controller.ToUpper().Equals(controller.ToUpper()))
                {
                    return true;
                }

            }


            return false;

        }





    }
}
