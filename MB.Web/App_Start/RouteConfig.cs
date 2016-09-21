using System.Web.Mvc;
using System.Web.Routing;

namespace TargetNavigation
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "V1.2",
                url: "XXXViews/{controller}/{action}/{id}",
                defaults: new { controller = "UserIndex", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "MB.Web.Controllers.NewControllers" }
            );

            //routes.MapRoute(
            //    name: "plan",
            //    url: "Plan/Index/{id}",
            //    defaults: new { controller = "Plan", action = "Index", id = UrlParameter.Optional },
            //    namespaces: new string[] { "MB.Web.Controllers" }
            //);

            //routes.MapRoute(
            //    name: "Flow",
            //    url: "FlowIndex/FlowIndex/{id}",
            //    defaults: new { controller = "FlowIndex", action = "FlowIndex", id = UrlParameter.Optional },
            //    namespaces: new string[] { "MB.Web.Controllers" }
            //);

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Login", action = "Login", id = UrlParameter.Optional},
                namespaces: new string[] { "MB.Web.Controllers" }
            );
        }
    }
}