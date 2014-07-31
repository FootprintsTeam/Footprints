using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Footprints.App_Start;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Util;
using Autofac.Features;
using AutoMapper;
using Footprints.Mappings;
using Footprints.Common;
namespace Footprints
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //Autofac setup
            DependencyResolver.SetResolver(new AutofacDependencyResolver(Bootstrapper.SetAutofacContainer()));
            //Mapper setup
            AutoMapperConfiguration.Configure();            
        }
    }
}
