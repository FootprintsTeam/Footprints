using System.Web;
using System.Web.Optimization;

namespace Footprints
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundle/jquerymig").Include(
                "~/Scripts/jquery-migrate-{version}.min.js"));

            bundles.Add(new ScriptBundle("~/bundle/nanogalleryscript").Include("~/Scripts/jquery.nanogallery.min.js", "~/Script/jquery.nanogallery.js"));
            bundles.Add(new StyleBundle("~/Content/nanogallerycss").Include("~/Content/nanogallerycss/nanogallery.css"));

            bundles.Add(new ScriptBundle("~/bundle/destinationscript").Include("~/Scripts/destinationscript.js"));
            bundles.Add(new StyleBundle("~/Content/destinationcss").Include("~/Content/destinationcss.css"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css"
                      ));
        }
    }
}
