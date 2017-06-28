using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WebApiRestfulBestPractice.Version
{
    /// <summary>
    /// 2 種設定版本的方式: Version / VersionAssembly
    /// </summary>
    public class VersioningMiddlewareOptions
    {
        /// <summary>
        /// 指定版本文字 - 優先採用
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 自動取得 AssemblyFileVersion
        /// </summary>
        public Assembly VersionAssembly { get; set; }

        /// <summary>
        /// 自定 Header 名稱
        /// </summary>
        public string HeaderName { get; set; }
    }
}
