using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace WebApiRestfulBestPractice.SnakeCase
{
    /// <summary>
    /// 將 Uri 參數換為 snake_case
    /// </summary>
    public class SnakeCaseActionSelector : ApiControllerActionSelector
    {
        public override HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)
        {
            var newUri = CreateNewUri(
                controllerContext.Request.RequestUri,
                controllerContext.Request.GetQueryNameValuePairs());

            controllerContext.Request.RequestUri = newUri;

            return base.SelectAction(controllerContext);
        }

        private Uri CreateNewUri(Uri requestUri, IEnumerable<KeyValuePair<string, string>> queryPairs)
        {
            var currentQuery = requestUri.Query;

            if (!currentQuery.Any())
            {
                return requestUri;
            }

            var newQuery = ConvertQueryToCamelCase(queryPairs);

            return new Uri(requestUri.ToString().Replace(currentQuery, newQuery));
        }

        private static string ConvertQueryToCamelCase(IEnumerable<KeyValuePair<string, string>> queryPairs)
        {
            queryPairs = queryPairs
                .Select(x => new KeyValuePair<string, string>(ToCamelCase(x.Key), x.Value));

            return "?" + queryPairs
                .Select(x => string.Format("{0}={1}", x.Key, x.Value))
                .Aggregate((x, y) => x + "&" + y);
        }

        private static string ToCamelCase(string source)
        {
            var parts = source
                .Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);

            return parts
                .First().ToLower() +
                string.Join("", parts.Skip(1).Select(ToCapital));
        }

        private static string ToCapital(string source)
        {
            return string.Format("{0}{1}", char.ToUpper(source[0]), source.Substring(1).ToLower());
        }
    }
}
