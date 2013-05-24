using System;
using Nancy.ViewEngines.Razor;

namespace RaccoonBlog.NancyFE.Helpers
{
    public interface IBundlesHelper
    {
        void Reference(string assetPathOrBundlePathOrUrl, string pageLocation = null);

        IHtmlString RenderScripts(string pageLocation = null);

        IHtmlString RenderStylesheets(string pageLocation = null);

        string FileUrl(string bundlePath);
        void AddInlineScript(string scriptContent, string pageLocation = null);
        void AddInlineScript(Func<object, object> scriptContent, string pageLocation = null);
    }
}