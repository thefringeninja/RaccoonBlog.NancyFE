using System;
using System.Collections.Generic;
using Nancy;
using Nancy.Responses.Negotiation;
using Nancy.ViewEngines;

namespace RaccoonBlog.NancyFE.Negotiation
{
    public class RssResponseProcessor : IResponseProcessor
    {
        private readonly MediaRange rss = "application/rss+xml";
        private readonly IViewFactory viewFactory;

        public RssResponseProcessor(IViewFactory viewFactory)
        {
            this.viewFactory = viewFactory;
        }

        #region IResponseProcessor Members

        public ProcessorMatch CanProcess(MediaRange requestedMediaRange, dynamic model, NancyContext context)
        {
            if (requestedMediaRange.Matches(rss))
            {
                return new ProcessorMatch
                {
                    ModelResult = MatchResult.DontCare,
                    RequestedContentTypeResult = MatchResult.ExactMatch
                };
            }
            return new ProcessorMatch
            {
                ModelResult = MatchResult.DontCare,
                RequestedContentTypeResult = MatchResult.NoMatch
            };
        }

        public Response Process(MediaRange requestedMediaRange, dynamic model, NancyContext context)
        {
            return viewFactory.RenderView(
                context.NegotiationContext.ViewName, (object)model,
                GetViewLocationContext(context))
                              .WithContentType(rss);

        }

        public IEnumerable<Tuple<string, MediaRange>> ExtensionMappings
        {
            get { yield return Tuple.Create("rss", rss); }
        }

        #endregion

        private static ViewLocationContext GetViewLocationContext(NancyContext context)
        {
            return new ViewLocationContext
            {
                Context = context,
                ModuleName = context.NegotiationContext.ModuleName,
                ModulePath = context.NegotiationContext.ModulePath + "/rss"
            };
        }
    }


}