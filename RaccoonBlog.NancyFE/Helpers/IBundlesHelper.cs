using Nancy.ViewEngines.Razor;

namespace RaccoonBlog.NancyFE.Helpers
{
    public interface IBundlesHelper
    {
        void Reference(string assetPathOrBundlePathOrUrl, string pageLocation = null);

        IHtmlString RenderScripts(string pageLocation = null);

        IHtmlString RenderStylesheets(string pageLocation = null);

        string FileUrl(string bundlePath);
    }
}