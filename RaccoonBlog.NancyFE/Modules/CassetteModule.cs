using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Cassette;
using Nancy;
using Nancy.Responses;
using RaccoonBlog.NancyFE.Startup;

namespace RaccoonBlog.NancyFE.Modules
{
    public class CassetteModule : NancyModule
    {
        private readonly HashAlgorithm HashAlgorithm = MD5.Create();

        private static readonly IDictionary<String, Func<String, BundleCollection, Bundle>> BundleFinders
            = new Dictionary<string, Func<String, BundleCollection, Bundle>>
                {
                    {"script", (path, bundles) => bundles.FindBundlesContainingPath(path).FirstOrDefault()},
                    {"stylesheet", (path, bundles) => bundles.FindBundlesContainingPath(path).FirstOrDefault()},
                    {"htmltemplate", (path, bundles) => bundles.FindBundlesContainingPath(path).FirstOrDefault()}
                };

        public CassetteModule(IProvideBundleCollections bundleProvider, IRootPathProvider rootPathProvider)
            : base(CassetteStartup.ModulePath)
        {
            Get["/"] = _ => 501;

            Get["/asset/{path}"]
                = p =>
                {
                    var bundles = bundleProvider.Provide();
                    var path = "~/" + (String)p.path;

                    using (bundles.GetReadLock())
                    {
                        IAsset asset;
                        Bundle bundle;
                        return false == bundles.TryGetAssetByPath(path, out asset, out bundle)
                                   ? new HtmlResponse(HttpStatusCode.NotFound)
                                   : HandleResourceRequest(asset.OpenStream, bundle.ContentType, asset.Hash);
                    }
                };

            Get["/file/{path}"]
                = p =>
                {
                    var path = "/" + (String)p.path;

                    var match = Regex.Match(path, "^(?<filename>.*)-[a-z0-9]+\\.(?<extension>[a-z]+)$", RegexOptions.IgnoreCase);
                    if (false == match.Success)
                    {
                        return new HtmlResponse(HttpStatusCode.InternalServerError);
                    }

                    var filePath = rootPathProvider.GetRootPath() + "\\" + match.Groups["filename"].Value.Replace('/', '\\') + "."
                                   + match.Groups["extension"].Value;
                    filePath = Regex.Replace(filePath, "\\\\{2,}", "\\");
                    if (false == File.Exists(filePath))
                    {
                        return new HtmlResponse(HttpStatusCode.NotFound);
                    }

                    var hash = HashAlgorithm.ComputeHash(File.ReadAllBytes(filePath));

                    return HandleResourceRequest(() => File.OpenRead(filePath), MimeTypes.GetMimeType(filePath), hash);
                };

            Get["/{bundleType}/{id}/{path*}"]
                = p =>
                {
                    var path = "~/" + (String)p.path;

                    Func<string, BundleCollection, Bundle> find;
                    if (false == BundleFinders.TryGetValue((String)p.bundleType, out find))
                    {
                        return new HtmlResponse(HttpStatusCode.NotFound);
                    }

                    var bundles = bundleProvider.Provide();

                    using (bundles.GetReadLock())
                    {
                        var bundle = find(path, bundles);
                        return bundle == null
                                   ? new HtmlResponse(HttpStatusCode.NotFound)
                                   : HandleResourceRequest(bundle.OpenStream, bundle.ContentType, bundle.Hash);
                    }
                };
        }

        private Response HandleResourceRequest(Func<Stream> resource, string contentType, IEnumerable<byte> hash)
        {
            var etag = "\"" + hash.ToHexString() + "\"";
            if (Request.Headers.IfNoneMatch.Contains(etag))
            {
                return HttpStatusCode.NotModified;
            }

            return new StreamResponse(resource, contentType)
                .WithHeader("ETag", etag)
                .WithHeader("Expires", DateTime.UtcNow.Add(TimeSpan.FromDays(365)).ToString("r"))
                .WithHeader("Cache-Control", "public, max-age=31536000");
        }
    }
}