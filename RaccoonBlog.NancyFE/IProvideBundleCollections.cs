using Cassette;

namespace RaccoonBlog.NancyFE
{
    public interface IProvideBundleCollections
    {
        BundleCollection Provide();
    }
}