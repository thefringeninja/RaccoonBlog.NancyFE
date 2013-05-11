using System;

namespace RaccoonBlog.NancyFE.Model
{
    public class Post
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string[] Tags { get; set; }
        public string[] TagsAsSlugs { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime PublishAt { get; set; }
        public string ContentType { get; set; }
    }
}