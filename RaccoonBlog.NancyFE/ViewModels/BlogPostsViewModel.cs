using System.Collections.Generic;
using RaccoonBlog.NancyFE.Model;

namespace RaccoonBlog.NancyFE.ViewModels
{
    public class BlogPostsViewModel
    {
        public BlogPostsViewModel(IEnumerable<Post> blogPosts)
        {
            BlogPosts = blogPosts;
        }

        public IEnumerable<Post> BlogPosts { get; private set; }
    }
}