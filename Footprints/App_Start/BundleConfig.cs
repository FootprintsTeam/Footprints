﻿using System.Web;
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
            bundles.Add(new StyleBundle("~/mycss").Include("~/Content/style.css", "~/Content/font-awesome.min.css"));

            //Template scripts            
            bundles.Add(new ScriptBundle("~/script/core").Include("~/assets/library/jquery/jquery.min.js", "~/assets/library/modernizr/modernizr.js"));
            bundles.Add(new ScriptBundle("~/script/dependency").Include(
                "~/assets/library/bootstrap/js/bootstrap.min.js",
                "~/assets/library/jquery/jquery-migrate.min.js",
                "~/assets/components/maps_google/maps-google.init.js"
                ));
            bundles.Add(new ScriptBundle("~/script/plugins").Include(
                //"~/assets/plugins/core_nicescroll/jquery.nicescroll.min.js",
                "~/assets/plugins/core_breakpoints/breakpoints.js",
                "~/assets/plugins/core_ajaxify_davis/davis.min.js",
                "~/assets/plugins/core_ajaxify_lazyjaxdavis/jquery.lazyjaxdavis.min.js",
                //"~/assets/plugins/core_preload/pace.min.js",
                "~/assets/plugins/menu_sidr/jquery.sidr.js",
                "~/assets/plugins/menu_sidr/jquery.sidr.js",
                "~/assets/plugins/media_holder/holder.js",
                "~/assets/plugins/media_gridalicious/jquery.gridalicious.min.js",
                //"~/assets/plugins/other_mixitup/jquery.mixitup.min.js",
                "~/assets/plugins/core_less-js/less.min.js",
                //"~/assets/plugins/charts_flot/excanvas.js",
                "~/assets/plugins/core_browser/ie/ie.prototype.polyfill.js"
                ));
            bundles.Add(new ScriptBundle("~/script/bundle").Include(
                "~/assets/components/core_ajaxify/ajaxify.init.js",
                //"~/assets/components/core_preload/preload.pace.init.js",
                //"~/assets/components/widget_twitter/twitter.init.js",
                "~/assets/components/media_gridalicious/gridalicious.init.js",
                //"~/assets/components/menus/sidebar.main.init.js",
                //"~/assets/components/menus/sidebar.collapse.init.js",
                //"~/assets/components/menus/menus.sidebar.chat.init.js",
                //"~/assets/plugins/other_mixitup/mixitup.init.js",
                "~/assets/components/core/core.init.js"));

            //Core css
            bundles.Add(new StyleBundle("~/css/core").Include(                
                "~/assets/css/admin/module.admin.stylesheet-complete.min.css"
                //, "~/assets/library/bootstrap/css/bootstrap.min.css"
                //, "~/assets/library/icons/fontawesome/assets/css/font-awesome.min.css"
                //, "~/assets/library/icons/glyphicons/assets/css/glyphicons_regular.css"
                //, "~/assets/library/icons/glyphicons/assets/css/glyphicons_social.css"
                //, "~/assets/library/icons/glyphicons/assets/css/glyphicons_filetypes.css"
                //, "~/assets/components/core/variables.less"
                //, "~/assets/components/core/mixins.less"
                //, "~/assets/components/core/scaffolding.less"
                //, "~/assets/components/core/helpers.less"
                ));

            //Timeline page css
            bundles.Add(new StyleBundle("~/css/timeline").Include(
                "~/assets/components/ui_buttons/buttons.less",
                "~/assets/library/icons/pictoicons/css/picto.css",
                "~/assets/components/core/widgets.less",
                "~/assets/components/ui_tabs/tabs.less",
                "~/assets/components/ui_buttons/buttons.less",
                "~/assets/components/ui_forms/forms.less",
                "~/assets/components/ui_ribbons/ribbons.less",
                "~/assets/components/admin_ratings/rating.less"));  
        }
    }
}
