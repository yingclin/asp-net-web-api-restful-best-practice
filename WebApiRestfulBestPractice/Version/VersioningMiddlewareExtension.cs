using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WebApiRestfulBestPractice.Version
{
    /// <summary>
    /// 於 Response Header 設定 Api Version
    /// </summary>
    public static class VersioningMiddlewareExtension
    {
        /// <param name="app"></param>
        public static void UseVersioningMiddleware(this IAppBuilder app, VersioningMiddlewareOptions options = null)
        {
            app.Use<VersioningMiddleware>(options);
        }
    }
}
