using System.Web;
using System.Web.Optimization;

namespace DigitalnaApoteka
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css"));

            bundles.Add(new StyleBundle("~/ContentDoctor/css").Include("~/Content/bootstrap.css",
                      "~/Content/Doctor/DoctorStyle.css"));
            bundles.Add(new StyleBundle("~/ContentPharmacist/css").Include("~/Content/bootstrap.css",
                      "~/Content/Pharmacist/PharmacistStyle.css"));
            bundles.Add(new StyleBundle("~/ContentPatient/css").Include("~/Content/bootstrap.css",
                      "~/Content/Patient/PatientStyle.css"));
            bundles.Add(new StyleBundle("~/ContentAdmin/css").Include("~/Content/bootstrap.css",
                      "~/Content/Admin/AdminStyle.css"));
        }
    }
}
