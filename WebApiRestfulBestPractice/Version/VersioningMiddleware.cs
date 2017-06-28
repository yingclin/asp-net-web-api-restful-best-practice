using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using WebApiRestfulBestPractice.Utils;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

namespace WebApiRestfulBestPractice.Version
{
    /// <summary>
    /// 於 Response Header 中自動以 Assembly file version 提供 Api Version
    /// </summary>
    class VersioningMiddleware
    {
        // 簡單保存在 static 中
        private static string ApiVersion;
        //
        private static string HeaderName = "Api-Version";
        //
        private AppFunc next;

        /// <param name="next"></param>
        public VersioningMiddleware(AppFunc next, VersioningMiddlewareOptions options)
        {
            this.next = next;
            //
            InitVersion(options);

            if (options != null && !string.IsNullOrEmpty(options.HeaderName))
            {
                HeaderName = options.HeaderName;
            }
        }

        /// <param name="env"></param>
        /// <returns></returns>
        public async Task Invoke(IDictionary<string, object> env)
        {
            var ctx = new OwinContext(env);

            if (ApiVersion != null)
            {
                ctx.Response.Headers.Add(HeaderName, new string[] { ApiVersion });
            }

            await this.next(env);
        }

        private void InitVersion(VersioningMiddlewareOptions options)
        {
            if (ApiVersion == null && options != null)
            {
                ApiVersion = GetVersionFromOptionsOrNull(options);
            }

            if(ApiVersion == null)
            {
                ApiVersion = GetVersionFromAssemblies();
            }
        }

        private string GetVersionFromOptionsOrNull(VersioningMiddlewareOptions options)
        {
            if(options.Version != null)
            {
                return options.Version;
            }

            if(options.VersionAssembly != null)
            {
                return GetFileVersion(options.VersionAssembly);
            }

            return null;
        }

        private string GetFileVersion(Assembly assembly)
        {
            var fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location);
            // Major.Minor.Build
            return string.Format("{0}.{1}.{2}", fileVersion.ProductMajorPart, fileVersion.ProductMinorPart, fileVersion.ProductBuildPart);
        }

        private string GetVersionFromAssemblies()
        {
            var assembies = AssemblyFinder.LookupContains(AssemblyFinder.GetReferencedAssemblies(), typeof(ApiController));

            if(assembies.Any())
            {
                return GetFileVersion(assembies.First());
            }

            return null;
        }
    }
}
