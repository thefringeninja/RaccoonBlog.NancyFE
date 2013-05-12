using System;
using System.Linq;
using RaccoonBlog.NancyFE.Model;

namespace RaccoonBlog.NancyFE.ViewModels
{
    public class BlogPostSummaryViewModel
    {
        public BlogPostSummaryViewModel(Post post, User author)
        {
            Body = String.Join(Environment.NewLine, post.Body.Split(
                new[] {"\r", "\n"}, StringSplitOptions.RemoveEmptyEntries).Take(3));
            PublishAt = post.PublishAt;
            Title = post.Title;
            Author = author.TwitterNick;
            Path = post.Id.Split('/').Last() + "/" + post.Title.Slugify();
        }

        public string Body { get; private set; }
        public DateTime PublishAt { get; private set; }

        public string Author { get; set; }

        public string Title { get; private set; }

        public string Path { get; private set; }
    }
}