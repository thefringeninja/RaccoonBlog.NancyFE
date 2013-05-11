using RaccoonBlog.NancyFE.Model;

namespace RaccoonBlog.NancyFE.ViewModels
{
    public class BlogPostViewModel
    {
        public BlogPostViewModel(Post post)
        {
            Post = post;
        }

        public Post Post { get; private set; }
    }
}