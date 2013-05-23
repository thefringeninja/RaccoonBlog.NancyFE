using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RaccoonBlog.NancyFE.Model;

namespace RaccoonBlog.NancyFE.ViewModels
{
    public class BlogPostsViewModel : IEnumerable<BlogPostSummaryViewModel>, IPageLayout
    {
        private readonly BlogConfig blogConfig;
        private readonly IEnumerable<BlogPostSummaryViewModel> blogPosts;

        public BlogPostsViewModel(IEnumerable<Post> posts, IEnumerable<Tag> tags, BlogConfig blogConfig, Func<Post, User> getAuthorOfPost)
        {
            this.blogConfig = blogConfig;
            blogPosts = (from post in posts
                         select new BlogPostSummaryViewModel(post, getAuthorOfPost(post)))
                .ToList();
            Tags = tags.Select(tag => new TagViewModel(tag)).ToList();
        }

        #region IEnumerable<BlogPostSummaryViewModel> Members

        public IEnumerator<BlogPostSummaryViewModel> GetEnumerator()
        {
            return blogPosts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IPageLayout Members

        string IPageLayout.Title
        {
            get { return blogConfig.Title; }
        }

        string IPageLayout.Subtitle
        {
            get { return blogConfig.Subtitle; }
        }

        string IPageLayout.Copyright
        {
            get { return blogConfig.Copyright; }
        }

        string IPageLayout.Description
        {
            get { return blogConfig.MetaDescription; }
        }

        public IEnumerable<TagViewModel> Tags { get; private set; }

        #endregion
    }
}