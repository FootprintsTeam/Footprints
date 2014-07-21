using System.Web.Mvc;

namespace Footprints.Areas.Media
{
    public class MediaAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Media";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Media_default",
                "Media/{controller}/{action}/{id}",
                new { controller="Media",action="Index",id=UrlParameter.Optional}
                );
        }
    }
}