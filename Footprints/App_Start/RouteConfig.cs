using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Footprints
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional },
                namespaces: new []{"Footprints.Controllers"}
            );

            routes.MapRoute(
                name: "Journey",
                url: "{controller}/{action}/{username}",
                defaults: new { controller = "Journey", action = "Index", username = UrlParameter.Optional},
                namespaces: new[] { "Footprints.Controllers" }
            );
        }
    }
}
