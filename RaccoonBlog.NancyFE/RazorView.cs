namespace RaccoonBlog.NancyFE
{
    public abstract class RazorView<TModel> : RazorViewBase<TModel>
    {
        protected RazorView()
        {
            Layout = "_layout.cshtml";
        }
    }
}