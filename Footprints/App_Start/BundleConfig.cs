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
                //"~/assets/library/bootstrap/css/bootstrap.min.css"
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

            //Personal page
            bundles.Add(new StyleBundle("~/css/personal").Include(
                "~/assets/components/admin_timeline/timeline-cover.less",
                "~/assets/plugins/media_blueimp/css/blueimp-gallery.min.css",
                "~/assets/components/media_blueimp/blueimp-gallery.less",
                "~/assets/components/admin_timeline/timeline.less",
                "~/assets/components/ui_tabs/tabs.less",
                "~/assets/components/ui_buttons/buttons.less",
                "~/assets/components/core/widgets.less",
                "~/assets/components/ui_media/gallery.less",
                "~/assets/components/widget_generic/widget-generic.less",
                "~/assets/components/core/widgets.less",
                "~/assets/library/icons/pictoicons/css/picto.css"
                ));

            bundles.Add(new ScriptBundle("~/script/personal").Include(
                "~/assets/plugins/media_blueimp/js/blueimp-gallery.min.js",
                "~/assets/plugins/media_blueimp/js/jquery.blueimp-gallery.min.js"
                ));

            //Destination Page
            bundles.Add(new StyleBundle("~/script/destination").Include(
                "~/assets/components/widget_lists/list-group.less",
				"~/assets/components/ui_labels/labels.less"
            ));

            //Media
            bundles.Add(new StyleBundle("~/css/media").Include(
                "~/assets/plugins/media_blueimp/css/blueimp-gallery.min.css",
                "~/assets/components/media_blueimp/blueimp-gallery.less"
            ));
            bundles.Add(new ScriptBundle("~/script/media").Include(
               "~/assets/plugins/media_blueimp/js/blueimp-gallery.min.js",
               "~/assets/plugins/media_blueimp/js/jquery.blueimp-gallery.min.js"
               ));

            //Journey Page
            bundles.Add(new StyleBundle("~/css/journey").Include(
                "~/assets/components/admin_timeline/timeline-cover.less",
                "~/assets/plugins/media_blueimp/css/blueimp-gallery.min.css",
                "~/assets/components/media_blueimp/blueimp-gallery.less",
                "~/assets/components/admin_timeline/timeline.less",
                "~/assets/components/ui_tabs/tabs.less",
                "~/assets/components/ui_buttons/buttons.less",
                "~/assets/components/core/widgets.less",
                "~/assets/components/ui_media/gallery.less",
                "~/assets/components/widget_generic/widget-generic.less",
                "~/assets/components/widget_lists/list-group.less",
                "~/assets/plugins/google_map/css/google.map.css"
                ));

            bundles.Add(new ScriptBundle("~/script/journey").Include(
                "~/assets/plugins/media_blueimp/js/blueimp-gallery.min.js",
                "~/assets/plugins/media_blueimp/js/jquery.blueimp-gallery.min.js"
                ));
			//Form-modal
            bundles.Add(new StyleBundle("~/css/form-modal").Include(
                "~/assets/components/ui_forms/forms.less",
                "~/assets/components/ui_buttons/buttons.less",
                "~/assets/components/forms_elements_fuelux-checkbox/fuelux-checkbox.less",
                "~/assets/plugins/notifications_gritter/css/jquery.gritter.css",
                "~/assets/components/admin_notifications_gritter/gritter.less",
                "~/assets/components/ui_modals/modals.less",
                "~/assets/components/ui_modals/modal-inline.less"
                ));
            bundles.Add(new ScriptBundle("~/script/form-modal").Include(
                "~/assets/components/forms_elements_fuelux-checkbox/fuelux-checkbox.init.js",
                "~/assets/plugins/ui_modals/bootbox.min.js",
                "~/assets/components/ui_modals/modals.init.js",
                "~/assets/plugins/notifications_gritter/js/jquery.gritter.min.js",
                "~/assets/components/admin_notifications_gritter/gritter.init.js"
               ));
            

            //Datepicker
            bundles.Add(new StyleBundle("~/css/datepicker").Include(
                "~/assets/plugins/forms_elements_bootstrap-datepicker/css/bootstrap-datepicker.css",
                "~/assets/components/forms_elements_bootstrap-datepicker/bootstrap-datepicker.less"
            ));
            bundles.Add(new ScriptBundle("~/script/datepicker").Include(
               "~/assets/plugins/forms_elements_bootstrap-datepicker/js/bootstrap-datepicker.js",
               "~/assets/components/forms_elements_bootstrap-datepicker/bootstrap-datepicker.init.js"
               ));
        }
    }
}
