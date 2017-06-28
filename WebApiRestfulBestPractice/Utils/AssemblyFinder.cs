using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Compilation;
using System.Web.Http;

namespace WebApiRestfulBestPractice.Utils
{
    /// <summary>
    /// 類別查找器
    /// </summary>
    public class AssemblyFinder
    {
        /// <summary>
        /// 從一堆組件中,找出有指定類別(Type)子類的組件
        /// </summary>
        /// <param name="assemblies"></param>
        /// <param name="checker">判斷規則實作</param>
        /// <returns></returns>
        public static IEnumerable<Assembly> LookupContains(IEnumerable<Assembly> assemblies, Type superType)
        {
            var matchAssembies = new List<Assembly>();

            foreach (var assembly in assemblies)
            {
                var types = from t in assembly.GetExportedTypes()
                            where SuperTypeChecker(t, superType)
                            select t;

                if (types.Any())
                {
                    matchAssembies.Add(assembly);
                }
            }

            return matchAssembies;
        }

        private static bool SuperTypeChecker(Type type, Type superType)
        {
            return type.IsClass && type.IsSubclassOf(superType);
        }

        /// <summary>
        /// 取得不以System及Microsoft為開頭的全部組件
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetReferencedAssemblies()
        {
            return from a in BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToList()
                   where !a.FullName.StartsWith("System") && !a.FullName.StartsWith("Microsoft")
                   select a;
        }
    }
}
