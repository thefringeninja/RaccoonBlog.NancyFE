using System;
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
        private const int PageSize = 10;

        public DefaultModule(IDocumentSession session)
        {
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

                return View[new BlogPostsViewModel(posts, post => session.LoadIncluded<User>(post.AuthorId))];
            };

            Get["/tagged/{tag}"] = p =>
            {
                var posts = session.Query<Post>().Current()
                                   .Tagged((string) p.tag)
                                   .Take(PageSize)
                                   .ToArray();
                return View[new BlogPostsViewModel(posts, post => session.LoadIncluded<User>(post.AuthorId))];
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
                
                return View[new BlogPostViewModel(post, author)];
            };
        }
    }
}