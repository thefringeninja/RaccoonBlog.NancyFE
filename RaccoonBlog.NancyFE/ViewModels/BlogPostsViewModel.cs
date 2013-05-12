using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RaccoonBlog.NancyFE.Model;

namespace RaccoonBlog.NancyFE.ViewModels
{
    public class BlogPostsViewModel : IEnumerable<BlogPostSummaryViewModel>
    {
        private readonly IEnumerable<BlogPostSummaryViewModel> blogPosts;

        public BlogPostsViewModel(IEnumerable<Post> posts, Func<Post, User> getAuthorOfPost)
        {
            blogPosts = (from post in posts
                         select new BlogPostSummaryViewModel(post, getAuthorOfPost(post)))
                .ToList();
        }

        public IEnumerator<BlogPostSummaryViewModel> GetEnumerator()
        {
            return blogPosts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}