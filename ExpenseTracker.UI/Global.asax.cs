//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using System.Web.Optimization;
//using System.Web.Routing;

//namespace ExpenseTracker.UI
//{
//    public class MvcApplication : System.Web.HttpApplication
//    {
//        protected void Application_Start()
//        {
//            AreaRegistration.RegisterAllAreas();
//            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
//            RouteConfig.RegisterRoutes(RouteTable.Routes);
//            BundleConfig.RegisterBundles(BundleTable.Bundles);
//        }
//    }
//}




using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ExpenseTracker.UI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        //GLOBAL AUTH HANDLER
        protected void Application_BeginRequest()
        {
            try
            {
                var context = HttpContext.Current;

                // Skip login/register pages
                string url = context.Request.Url.AbsolutePath.ToLower();

                if (url.Contains("/account/login") || url.Contains("/account/register"))
                    return;

                // Check session token
                var session = context.Session;

                if (session == null) return;

                var token = session["Token"];

                if (token == null)
                {
                    context.Response.Redirect("/Account/Login");
                }
            }
            catch
            {
                // fallback safe
            }
        }
    }
}