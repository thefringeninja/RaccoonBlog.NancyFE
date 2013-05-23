using Nancy.ViewEngines;
using Nancy.ViewEngines.Razor;
using RaccoonBlog.NancyFE.Helpers;
using RaccoonBlog.NancyFE.Startup;

namespace RaccoonBlog.NancyFE
{
    public abstract class RazorViewBase<TModel> : NancyRazorViewBase<TModel>
    {
        public IBundlesHelper Bundles { get; private set; }

        public override void Initialize(RazorViewEngine engine, IRenderContext renderContext, object model)
        {
            base.Initialize(engine, renderContext, model);

            Bundles = (IBundlesHelper) renderContext.Context.Items[CassetteStartup.BUNDLES_HELPER];
        }
    }
}