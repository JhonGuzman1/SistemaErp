using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Datos;
using Logica;


namespace SistemaERP.Models
{
    public class Authorize : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorized = base.AuthorizeCore(httpContext);

            var routeData = httpContext.Request.RequestContext.RouteData;
            var controller = routeData.GetRequiredString("controller");
            var action = routeData.GetRequiredString("action");

            httpContext.Session.Remove("UsuarioTemp");

            httpContext.Items["CodigoError"] = 0;

            if (httpContext.Request.IsAjaxRequest())
            {

            }
            if (httpContext.Session["Usuario"] != null && (httpContext.Session["Bloqueado"] == null || !bool.Parse(httpContext.Session["Bloqueado"].ToString())))
            {
                try
                {
                    Usuario oUsuario = (Usuario)httpContext.Session["Usuario"];
                    Empresa empresa =  (Empresa)httpContext.Session["Empresa"];

                    try
                    {
                        var Result = LUsuario.Logica.LUsuario.IniciarSesion(oUsuario.Usuario1, oUsuario.Pass);
                        httpContext.Session["Usuario"] = Result;
                        oUsuario = (Usuario)httpContext.Session["Usuario"];

                    }
                    catch (Exception ex)
                    {
                        httpContext.Session.RemoveAll();
                        if (httpContext.Request.IsAjaxRequest())
                        {
                            httpContext.Session["Error"] = "Sus permisos han sido modificados, inicie sesión nuevamente";
                        }
                        return false;
                    }


                    if (LUsuario.Logica.LUsuario.PerimisosController(controller) && empresa != null)
                    {
                        
                        return true;
                    }
                    else
                    {
                        if (controller.ToUpper().Equals("EMPRESA"))
                        {
                            return true;
                        }
                        else
                        {
                            httpContext.Items["CodigoError"] = 403;
                            return false;
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                if ((httpContext.Session["Bloqueado"] != null && bool.Parse(httpContext.Session["Bloqueado"].ToString())))
                {
                    httpContext.Items["CodigoError"] = 444;
                }
                return false;
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            int CotasCodigoError = 0;
            if (filterContext.HttpContext.Items["CodigoError"] != null)
            {
                CotasCodigoError = Convert.ToInt32(filterContext.HttpContext.Items["CodigoError"].ToString());
            }

            string action = "Index";
            if (filterContext.HttpContext.Session["Bloqueado"] != null && bool.Parse(filterContext.HttpContext.Session["Bloqueado"].ToString()))
            {
                action = "Bloquear";
            }

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                var urlHelper = new UrlHelper(filterContext.RequestContext);
                filterContext.HttpContext.Response.StatusCode = 403;
                filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        Error = "NotAuthorized",
                        LogOnUrl = urlHelper.Action(action, "Login")
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                switch (CotasCodigoError)
                {
                    case 444:
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Login" }, { "action", action } });
                        break;
                    case 403:
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Login" }, { "action", action } });
                        break;
                    default:
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Login" }, { "action", action } });
                        base.HandleUnauthorizedRequest(filterContext);
                        break;
                }
            }
        }
    }
}