using System;
using System.Configuration;
using Nancy;
using Nancy.TinyIoc;
using RaccoonBlog.NancyFE.Startup;
using Raven.Client;
using Raven.Client.Document;

namespace RaccoonBlog.NancyFE.Bootstrapper
{
    public class Bootstrapper
        : DefaultNancyBootstrapper
    {
        public Bootstrapper()
        {
            CassetteStartup.OptimizeOutput = true;
        }
        private readonly Lazy<IDocumentStore> documentStoreLazy = new Lazy<IDocumentStore>(() =>
        {
            var documentStore = new DocumentStore();
            documentStore.ParseConnectionString(ConfigurationManager.AppSettings["RAVENHQ_CONNECTION_STRING"]);
            documentStore.Initialize();

            //IndexCreation.CreateIndexes(typeof(Bootstrapper).Assembly, documentStore);

            return documentStore;
        });

        private IDocumentStore DocumentStore
        {
            get { return documentStoreLazy.Value; }
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);

            container.Register((_, n) => DocumentStore.OpenSession());
        }
    }
}