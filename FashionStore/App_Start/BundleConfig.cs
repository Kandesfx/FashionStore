using System.Web;
using System.Web.Optimization;

namespace FashionStore
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Comment out bundles that require files that don't exist
            // Uncomment and install packages if needed (jQuery, Bootstrap, etc.)
            
            // bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //             "~/Scripts/jquery-{version}.js"));

            // bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //             "~/Scripts/jquery.validate*"));

            // bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //             "~/Scripts/modernizr-*"));

            // bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //           "~/Scripts/bootstrap.js"));

            // Only include CSS files that exist
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/site.css"));
        }
    }
}

