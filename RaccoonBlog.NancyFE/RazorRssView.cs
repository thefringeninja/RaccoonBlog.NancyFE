namespace RaccoonBlog.NancyFE
{
    public abstract class RazorRssView<TModel> : RazorViewBase<TModel>
    {
        protected RazorRssView()
        {
            Layout = "_rss-layout.cshtml";
        }
    }
}