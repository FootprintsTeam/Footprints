using System.Web.Mvc;

namespace Footprints.Areas.Journey
{
    public class JourneyAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Journey";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Journey_default",
                "Journey/{controller}/{action}/{id}",
                defaults: new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}