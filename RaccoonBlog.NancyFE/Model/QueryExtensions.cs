using System;
using System.Linq;
using Raven.Client;
using Raven.Client.Linq;

namespace RaccoonBlog.NancyFE.Model
{
    public static class QueryExtensions
    {
        public static IRavenQueryable<Post> Current(this IRavenQueryable<Post> posts)
        {
            return from post in posts.Customize(IncludeUsers)
                   orderby post.PublishAt
                   where false == post.IsDeleted
                         && post.PublishAt <= DateTime.UtcNow
                   select post;
        }

        private static void IncludeUsers(IDocumentQueryCustomization c)
        {
            c.Include<Post>(post => post.AuthorId);
        }

        public static IRavenQueryable<Post> Tagged(this IRavenQueryable<Post> posts, string tagSlug)
        {
            return from post in posts.Customize(IncludeUsers)
                   where post.TagsAsSlugs.Any(tag => tag == tagSlug)
                   select post;
        }
    }
}