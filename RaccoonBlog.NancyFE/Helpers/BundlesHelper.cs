using System;
using Cassette;
using Cassette.Scripts;
using Cassette.Stylesheets;
using Nancy.ViewEngines.Razor;

namespace RaccoonBlog.NancyFE.Helpers
{
    public class BundlesHelper : IBundlesHelper
    {
        private readonly IPlaceholderTracker placeholderTracker;
        private readonly IReferenceBuilder referenceBuilder;

        public BundlesHelper(IReferenceBuilder referenceBuilder, IPlaceholderTracker placeholderTracker)
        {
            this.referenceBuilder = referenceBuilder;
            this.placeholderTracker = placeholderTracker;
            
        }

        #region IBundlesHelper Members

        public void Reference(string assetPathOrBundlePathOrUrl, string pageLocation = null)
        {
            referenceBuilder.Reference(assetPathOrBundlePathOrUrl, pageLocation);
        }

        public IHtmlString RenderScripts(string pageLocation = null)
        {
            return Render<ScriptBundle>();
        }

        public IHtmlString RenderStylesheets(string pageLocation = null)
        {
            return Render<StylesheetBundle>();
        }
        public void AddInlineScript(Func<object, object> scriptContent, string pageLocation = null)
        {
            AddInlineScript(scriptContent(null).ToString(), pageLocation);
        }
        public void AddInlineScript(string scriptContent, string pageLocation = null)
        {
            var script = new InlineScriptBundle(scriptContent);
            referenceBuilder.Reference(script, pageLocation);
        }

        public string FileUrl(string bundlePath)
        {
            throw new NotImplementedException();
        }

        #endregion

        private IHtmlString Render<TBundle>(string pageLocation = null) where TBundle : Bundle
        {
            var html = placeholderTracker.ReplacePlaceholders(referenceBuilder.Render<TBundle>(pageLocation));
            return new NonEncodedHtmlString(html);
        }
    }
}