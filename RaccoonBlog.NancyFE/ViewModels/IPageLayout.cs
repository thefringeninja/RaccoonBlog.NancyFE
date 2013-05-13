namespace RaccoonBlog.NancyFE.ViewModels
{
    public interface IPageLayout
    {
        string Title { get; }

        string Subtitle { get; }

        string Copyright { get; }

        string Description { get; }
    }
}