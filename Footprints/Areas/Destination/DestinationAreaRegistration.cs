using System.Web.Mvc;

namespace Footprints.Areas.Destination
{
    public class DestinationAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Destination";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Destination_default",
                "Destination/{controller}/{action}/{id}",
                new {controller = "Destination", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}