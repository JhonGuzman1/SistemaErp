using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datos;

namespace Entidad
{
    public class ESesion
    {

        public static T ObtenerSesion<T>(object sesion) where T : Usuario
        {
            T oUsuario = null;
            if (sesion != null)
            {
                try
                {
                    oUsuario = (T)sesion;
                }
                catch
                {
                    oUsuario = null;
                }
            }
            return oUsuario;

        }


        public static T ObtenerEmpresa<T>(object sesion) where T : Empresa
        {
            T oUsuario = null;
            if (sesion != null)
            {
                try
                {
                    oUsuario = (T)sesion;
                }
                catch
                {
                    oUsuario = null;
                }
            }
            return oUsuario;

        }

        public static T ObtenerEmpresaSelect<T>(object sesion) where T : Empresa
        {
            T oUsuario = null;
            if (sesion != null)
            {
                try
                {
                    oUsuario = (T)sesion;
                }
                catch
                {
                    oUsuario = null;
                }
            }
            return oUsuario;

        }


        public static T ObtenerGestion<T>(object sesion) where T : Gestion
        {
            T entidad = null;
            if (sesion != null)
            {
                try
                {
                    entidad = (T)sesion;
                }
                catch
                {
                    entidad = null;
                }
            }
            return entidad;

        }
    }
}
