using Raven.Abstractions.Data;
using Raven.Client;

namespace RaccoonBlog.NancyFE
{
    public static class DocumentStoreExtensions
    {
        public static void DeleteAll<T>(this IDocumentStore documentStore)
        {
            documentStore.DatabaseCommands.DeleteByIndex("Raven/DocumentsByEntityName", new IndexQuery
            {
                Query = "Tag: " + typeof (T).Name
            });
        }
    }
}