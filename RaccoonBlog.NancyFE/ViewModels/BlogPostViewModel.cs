using System;
using RaccoonBlog.NancyFE.Model;

namespace RaccoonBlog.NancyFE.ViewModels
{
    public class BlogPostViewModel
    {
        public BlogPostViewModel(Post post, User author)
        {
            Body = post.Body;
            PublishAt = post.PublishAt;
            Title = post.Title;
            Author = author.TwitterNick;
        }

        public string Body { get; private set; }
        public DateTime PublishAt { get; private set; }

        public string Author { get; set; }

        public string Title { get; private set; }
    }
}