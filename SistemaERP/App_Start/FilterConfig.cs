using System.Web;
using System.Web.Mvc;
using SistemaERP.Models;

namespace SistemaERP
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new Authorize());
            filters.Add(new HandleErrorAttribute());
        }
    }
}
