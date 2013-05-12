using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using Cassette;
using Cassette.IO;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using IsolatedStorageFile = System.IO.IsolatedStorage.IsolatedStorageFile;
using TinyIoCContainer = Cassette.TinyIoC.TinyIoCContainer;

namespace RaccoonBlog.NancyFE.Startup
{
    public class CassetteStartup : HostBase, IApplicationStartup, IProvideBundleCollections
    {
        private readonly Nancy.TinyIoc.TinyIoCContainer nancyContainer;
        private readonly IRootPathProvider rootPathProvider;

        static CassetteStartup()
        {
            ModulePath = "/_cassette";
        }

        public CassetteStartup(IRootPathProvider rootPathProvider, Nancy.TinyIoc.TinyIoCContainer nancyContainer)
        {
            this.rootPathProvider = rootPathProvider;
            this.nancyContainer = nancyContainer;
            AppDomainAssemblyTypeScanner.LoadAssemblies("Cassette.CoffeeScript.dll");
            AppDomainAssemblyTypeScanner.LoadAssemblies("Cassette.Hogan.dll");
            AppDomainAssemblyTypeScanner.LoadAssemblies("Cassette.JQueryTmpl.dll");
            AppDomainAssemblyTypeScanner.LoadAssemblies("Cassette.KnockoutJQueryTmpl.dll");
            AppDomainAssemblyTypeScanner.LoadAssemblies("Cassette.Less.dll");
            AppDomainAssemblyTypeScanner.LoadAssemblies("Cassette.Sass.dll");
            AppDomainAssemblyTypeScanner.LoadAssemblies("Cassette.dll");
        }

        public static bool OptimizeOutput { get; set; }

        protected override bool CanCreateRequestLifetimeProvider
        {
            get { return true; }
        }

        public static string ModulePath { get; set; }

        #region IApplicationStartup Members

        public void Initialize(IPipelines pipelines)
        {
            Initialize();
            //pipelines.AfterRequest.AddItemToEndOfPipeline(RewriteResponseContents);
        }

        #endregion

        #region IProvideBundleCollections Members

        BundleCollection IProvideBundleCollections.Provide()
        {
            return Container.Resolve<BundleCollection>();
        }

        #endregion

        public void RewriteResponseContents(NancyContext context)
        {
            if (false == context.Response.ContentType.Equals("text/html"))
            {
                // Only html needs to be (possibly) rewritten
                return;
            }
            var currentContents = context.Response.Contents;
            context.Response.Contents =
                    stream =>
                    {
                        var placeholderTracker = Container.Resolve<IPlaceholderTracker>();

                        var currentContentsStream = new MemoryStream();
                        currentContents(currentContentsStream);
                        var reader = new StreamReader(currentContentsStream);

                        reader.BaseStream.Seek(0, SeekOrigin.Begin);
                        var writer = new StreamWriter(stream);

                        writer.Write(placeholderTracker.ReplacePlaceholders(reader.ReadToEnd()));

                        writer.Flush();
                    };
        }

        protected override IEnumerable<Assembly> LoadAssemblies()
        {
            return AppDomainAssemblyTypeScanner.Assemblies;
        }

        protected override IConfiguration<CassetteSettings> CreateHostSpecificSettingsConfiguration()
        {
            return new CassetteConfiguration(rootPathProvider);
        }

        protected override TinyIoCContainer.ITinyIoCObjectLifetimeProvider CreateRequestLifetimeProvider()
        {
            return new HttpContextLifetimeProvider();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.Register(rootPathProvider);
            Container.Register<IUrlModifier>((container, _) => new UrlModifier());
            Container.Register<IUrlGenerator>((container, _) => new UrlGenerator(container.Resolve<IUrlModifier>(), ModulePath + "/"));

            nancyContainer.Register(ResolveFromCassetteContainer<IFileSearchProvider>());
            nancyContainer.Register(ResolveFromCassetteContainer<IBundleFactoryProvider>());
            nancyContainer.Register(ResolveFromCassetteContainer<CassetteSettings>());
            nancyContainer.Register<IProvideBundleCollections>(this);
        }

        private Func<
                Nancy.TinyIoc.TinyIoCContainer,
                NamedParameterOverloads,
                TRegisterType
                > ResolveFromCassetteContainer<TRegisterType>() where TRegisterType : class
        {
            return (_, __) => Container.Resolve<TRegisterType>();
        }

        #region Nested type: CassetteConfiguration

        private class CassetteConfiguration : IConfiguration<CassetteSettings>
        {
            private readonly IRootPathProvider rootPathProvider;
            private readonly CassetteConfigurationSection configuration;

            public CassetteConfiguration(IRootPathProvider rootPathProvider)
            {
                this.rootPathProvider = rootPathProvider;

                configuration = (WebConfigurationManager.GetSection("cassette") as CassetteConfigurationSection)
                                ?? new CassetteConfigurationSection();

            }

            #region IConfiguration<CassetteSettings> Members

            public void Configure(CassetteSettings configurable)
            {
                configurable.IsDebuggingEnabled = !OptimizeOutput;
                configurable.IsHtmlRewritingEnabled = true;
                configurable.SourceDirectory = new FileSystemDirectory(rootPathProvider.GetRootPath());
                configurable.CacheDirectory = GetCacheDirectory(configuration);
            }

            #endregion

            private IDirectory GetCacheDirectory(CassetteConfigurationSection configurationSection)
            {
                var path = configurationSection.CacheDirectory;
                if (string.IsNullOrEmpty(path))
                {
                    return new IsolatedStorageDirectory(IsolatedStorageFile.GetMachineStoreForAssembly);
                }
                if (Path.IsPathRooted(path))
                {
                    return new FileSystemDirectory(path);
                }
                path = path.TrimStart('~', '/');
                return new FileSystemDirectory(Path.Combine(rootPathProvider.GetRootPath(), path));
            }
        }

        #endregion

        #region Nested type: HttpContextLifetimeProvider

        /// <summary>
        ///   Hate to use a hosting specific provider here but we know we are going to use asp.net 
        ///   for the forseeable future.
        /// </summary>
        private class HttpContextLifetimeProvider : TinyIoCContainer.ITinyIoCObjectLifetimeProvider
        {
            private readonly string _KeyName = String.Format("TinyIoC.HttpContext.{0}", Guid.NewGuid());

            #region ITinyIoCObjectLifetimeProvider Members

            public object GetObject()
            {
                return HttpContext.Current.Items[_KeyName];
            }

            public void SetObject(object value)
            {
                HttpContext.Current.Items[_KeyName] = value;
            }

            public void ReleaseObject()
            {
                var item = GetObject() as IDisposable;

                if (item != null)
                    item.Dispose();

                SetObject(null);
            }

            #endregion
        }

        #endregion

        #region Nested type: UrlGenerator

        private class UrlGenerator : IUrlGenerator
        {
            private static readonly PropertyInfo urlProperty
                    = typeof(Bundle).GetProperty("Url", BindingFlags.Instance | BindingFlags.NonPublic);

            private readonly string cassetteHandlerPrefix;
            private readonly IUrlModifier urlModifier;

            public UrlGenerator(IUrlModifier urlModifier, string cassetteHandlerPrefix)
            {
                this.urlModifier = urlModifier;
                this.cassetteHandlerPrefix = cassetteHandlerPrefix;
            }

            #region IUrlGenerator Members

            public string CreateBundleUrl(Bundle bundle)
            {
                return urlModifier.Modify(cassetteHandlerPrefix + urlProperty.GetValue(bundle, null));
            }

            public string CreateAssetUrl(IAsset asset)
            {
                return urlModifier.Modify(cassetteHandlerPrefix + "asset" + asset.Path.Substring(1) + "?" + asset.Hash.ToUrlSafeBase64String());
            }

            public string CreateRawFileUrl(string filename, string hash)
            {
                if (!filename.StartsWith("~"))
                    throw new ArgumentException("Image filename must be application relative (starting with '~').");
                var str = ConvertToForwardSlashes(filename).Substring(1);
                var startIndex = str.LastIndexOf('.');
                return urlModifier.Modify(cassetteHandlerPrefix + "file" + (startIndex < 0 ? str + "-" + hash : str.Insert(startIndex, "-" + hash)));
            }

            public string CreateAbsolutePathUrl(string applicationRelativePath)
            {
                return urlModifier.Modify(applicationRelativePath.TrimStart('~', '/'));
            }

            #endregion

            private string ConvertToForwardSlashes(string path)
            {
                return path.Replace('\\', '/');
            }
        }

        #endregion

        #region Nested type: UrlModifier

        private class UrlModifier : IUrlModifier
        {
            private static readonly Regex what = new Regex("^(.*cassette.axd)", RegexOptions.Compiled);
            #region IUrlModifier Members

            public string Modify(string url)
            {
                return ConfigurationManager.AppSettings["CDN_STATIC_CONTENT"] + what.Replace(url, ModulePath);
            }

            #endregion
        }

        #endregion
    }
}
