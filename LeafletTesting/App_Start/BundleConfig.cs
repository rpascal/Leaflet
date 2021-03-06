﻿using System.Web;
using System.Web.Optimization;

namespace LeafletTesting
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                       "~/Scripts/jquery-3.3.1.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));


            bundles.Add(new StyleBundle("~/Content/leaflet").Include(
                "~/Content/leaflet/leaflet.css"));
            bundles.Add(new ScriptBundle("~/bundles/leaflet").Include(
                 "~/Scripts/leaflet/leaflet.js"));
            bundles.Add(new ScriptBundle("~/bundles/leafletExtensions").Include(
                 "~/Scripts/leaflet/LeafletExtensions/LeafletExtensions.js",
                 "~/Scripts/leaflet/LeafletExtensions/leaflet-bing-layer.js",
                 "~/Scripts/leaflet/LeafletExtensions/MapControls.js",
                 "~/Scripts/leaflet/LeafletExtensions/ObjectAssignPolyfill.js",
                   "~/Scripts/leaflet/LeafletExtensions/PromisesPolyfill.js"
                   
                 ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
        }
    }
}
