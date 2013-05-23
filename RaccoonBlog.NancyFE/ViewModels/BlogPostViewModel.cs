using System;
using RaccoonBlog.NancyFE.Model;

namespace RaccoonBlog.NancyFE.ViewModels
{
    public class BlogPostViewModel : IPageLayout
    {
        private readonly BlogConfig blogConfig;

        public BlogPostViewModel(Post post, User author, BlogConfig blogConfig)
        {
            Body = post.Body;
            PublishAt = post.PublishAt;
            PostTitle = post.Title;
            Author = author.TwitterNick;
            this.blogConfig = blogConfig;
        }

        public string Body { get; private set; }
        public DateTime PublishAt { get; private set; }

        public string Author { get; set; }

        public string PostTitle { get; private set; }

        #region IPageLayout Members

        string IPageLayout.Title
        {
            get { return blogConfig.Title + " - " + PostTitle; }
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

        #endregion
    }
}