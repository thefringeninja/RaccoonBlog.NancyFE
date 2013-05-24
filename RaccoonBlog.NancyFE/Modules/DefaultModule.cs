using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nancy.Responses;
using RaccoonBlog.NancyFE.Model;
using RaccoonBlog.NancyFE.ViewModels;
using Raven.Client;

namespace RaccoonBlog.NancyFE.Modules
{
    public class DefaultModule : NancyModule
    {
        private readonly IDocumentSession session;
        private const int PageSize = 10;

        protected BlogConfig GetBlogConfig()
        {
            // can probably even cache this for a whole day. It will not change much.
            using (session.Advanced.DocumentStore.AggressivelyCacheFor(TimeSpan.FromMinutes(30)))
            {
                return session.Load<BlogConfig>("Blog/Config");
            }
        }

        public DefaultModule(IDocumentSession session)
        {
            this.session = session;
            // 10 most recent blog posts
            Get["/"] = _ =>
            {
                DynamicDictionaryValue pageValue = Request.Query.page;
                var page = (pageValue.HasValue
                                ? pageValue
                                : 1) - 1;
                if (page < 0)
                {
                    return 400;
                }

                var posts = session.Query<Post>().Current()
                                   .Skip(page*PageSize)
                                   .Take(PageSize)
                                   .ToList();

                if (false == posts.Any())
                {
                    return 404;
                }

                return View[new BlogPostsViewModel(posts, GetTags(session), GetBlogConfig(), post => session.LoadIncluded<User>(post.AuthorId))];
            };

            Get["/tagged/{tag}"] = p =>
            {
                var posts = session.Query<Post>().Current()
                                   .Tagged((string) p.tag)
                                   .Take(PageSize)
                                   .ToArray();

                return View[new BlogPostsViewModel(posts, GetTags(session), GetBlogConfig(), post => session.LoadIncluded<User>(post.AuthorId))];
            };

            Get["/{id}/{slug}"] = p =>
            {
                Post post = session
                    .Include<Post>(x => x.AuthorId)
                    .Load<Post>("posts/" + p.id);
                
                if (post == null)
                {
                    return 404;
                }

                if (post.IsDeleted)
                {
                    return 410;
                }

                if (post.PublishAt > DateTime.UtcNow)
                {
                    return 403;
                }

                var slug = post.Title.Slugify();

                if (slug != (string) p.slug)
                {
                    return Response.AsRedirect(
                        "/" + (string) p.id + "/" + slug,
                        RedirectResponse.RedirectType.Permanent);
                }

                var author = session.LoadIncluded<User>(post.AuthorId);
                
                return View[new BlogPostViewModel(post, author, GetBlogConfig())];
            };
        }

        private static List<Tag> GetTags(IDocumentSession session)
        {
            using (session.Advanced.DocumentStore.AggressivelyCacheFor(TimeSpan.FromMinutes(30)))
            {
                var tags = session.Query<Tag>("Tags/Count", true)
                                  .OrderBy(tag => tag.Count)
                                  .ToList();
                return tags;
            }
        }
    }
}