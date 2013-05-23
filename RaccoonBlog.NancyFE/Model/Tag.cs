using System;

namespace RaccoonBlog.NancyFE.Model
{
    public class Tag
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public DateTimeOffset LastSeenAt { get; set; }
    }
}