using System.Web.Mvc;

namespace Footprints.Areas.Newsfeed
{
    public class NewsfeedAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Newsfeed";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Newsfeed_default",
                "Newsfeed1/{controller}/{action}/{id}",
                new { controller = "Newsfeed", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}