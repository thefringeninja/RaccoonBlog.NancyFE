using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Imports.Newtonsoft.Json;
using Raven.Json.Linq;

namespace RaccoonBlog.NancyFE
{
    public static class RavenStoreExtensions
    {
        public static void DeleteAll<T>(this IDocumentStore documentStore)
        {
            documentStore.DatabaseCommands.DeleteByIndex("Raven/DocumentsByEntityName", new IndexQuery
            {
                Query = "Tag: " + typeof (T).Name
            });
        }

        public static T LoadIncluded<T>(this IDocumentSession session, string id)
        {
            var document = session.Load<dynamic>(id);

            RavenJObject inner = document.Inner;
            
            using (JsonReader reader = new RavenJTokenReader(inner))
            {
                return session.Advanced.DocumentStore.Conventions.CreateSerializer().Deserialize<T>(reader);
            }
        }
    }
}