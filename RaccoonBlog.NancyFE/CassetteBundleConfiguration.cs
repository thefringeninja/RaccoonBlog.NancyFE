using Cassette;
using Cassette.Scripts;
using Cassette.Stylesheets;

namespace RaccoonBlog.NancyFE
{
    /// <summary>
    /// Configures the Cassette asset bundles for the web application.
    /// </summary>
    public class CassetteBundleConfiguration : IConfiguration<BundleCollection>
    {
        public void Configure(BundleCollection bundles)
        {
            bundles.Add<StylesheetBundle>("content/bootstrap/metro-bootstrap", new[] { "metro-bootstrap.less" });
            bundles.Add<StylesheetBundle>("content/bootstrap", new[] { "bootstrap.less" });

            bundles.Add<StylesheetBundle>("content/css", new[] { "main.less" });
            
        }
    }
}