using System;
using System.Linq;

namespace RaccoonBlog.NancyFE.Model
{
    public static class QueryExtensions
    {
        public static IQueryable<Post> Current(this IQueryable<Post> posts)
        {
            return from post in posts
                   orderby post.PublishAt
                   where false == post.IsDeleted
                         && post.PublishAt <= DateTime.UtcNow
                   select post;
        }

        public static IQueryable<Post> Tagged(this IQueryable<Post> posts, string tagSlug)
        {
            return from post in posts
                   where post.TagsAsSlugs.Any(tag => tag == tagSlug)
                   select post;
        }
    }
}