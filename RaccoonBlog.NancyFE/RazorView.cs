using Nancy.ViewEngines.Razor;

namespace RaccoonBlog.NancyFE
{
    public abstract class RazorView<TModel> : NancyRazorViewBase<TModel>
    {
        protected RazorView()
        {
            Layout = "_layout.cshtml";
        }
    }
}