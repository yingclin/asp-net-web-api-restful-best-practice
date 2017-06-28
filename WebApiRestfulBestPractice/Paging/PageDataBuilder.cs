using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Routing;

namespace WebApiRestfulBestPractice.Paging
{
    /// <summary>
    /// 產生分頁資訊: 頁碼及連結 Uri
    /// </summary>
    public class PageDataBuilder
    {
        public Uri FirstPage { get; private set; }
        public Uri LastPage { get; private set; }
        public Uri NextPage { get; private set; }
        public Uri PreviousPage { get; private set; }
        public bool HasLink { get; set; }
        //
        public int PageNo { get; private set; }
        public int PageSize { get; private set; }
        public int TotalRecordCount { get; private set; }
        public int PageCount { get; private set; }


        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="pageNo">需求的頁碼</param>
        /// <param name="pageSize">每頁的筆數</param>
        /// <param name="totalRecordCount">資料總筆數</param>
        /// <param name="requestPath">服務路徑 ex:Request.RequestUri.GetLeftPart(UriPartial.Path), 沒傳就不產生 Link</param>
        /// <param name="routeValues">Link 的 Url 參數, 匿名物件</param>
        public PageDataBuilder(int pageNo, 
            int pageSize,
            int totalRecordCount, 
            string requestPath = null,
            object routeValues = null)
        {
            // Determine total number of pages
            var pageCount = totalRecordCount > 0
                ? (int)Math.Ceiling(totalRecordCount / (double)pageSize)
                : 0;

            PageNo = pageNo;
            PageSize = pageSize;
            PageCount = pageCount;
            TotalRecordCount = totalRecordCount;

            // create page links
            if (!string.IsNullOrEmpty(requestPath))
            {
                HasLink = true;

                FirstPage = BuildUri(requestPath, new HttpRouteValueDictionary(routeValues)
                {
                    {"pageNo", 1},
                    {"pageSize", pageSize}
                });

                LastPage = BuildUri(requestPath, new HttpRouteValueDictionary(routeValues)
                {
                    {"pageNo", pageCount},
                    {"pageSize", pageSize}
                });

                if (pageNo > 1)
                {
                    PreviousPage = BuildUri(requestPath, new HttpRouteValueDictionary(routeValues)
                {
                    {"pageNo", pageNo - 1},
                    {"pageSize", pageSize}
                });
                }

                if (pageNo < pageCount)
                {
                    NextPage = BuildUri(requestPath, new HttpRouteValueDictionary(routeValues)
                {
                    {"pageNo", pageNo + 1},
                    {"pageSize", pageSize}
                });
                }
            }
        }

        /// <summary>
        /// 由 HttpRequestMessage 取得 requestPath
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns></returns>
        public static string GetUriPath(HttpRequestMessage request)
        {
            return request.RequestUri.GetLeftPart(UriPartial.Path);
        }

        private Uri BuildUri(string path, HttpRouteValueDictionary routeValues)
        {
            return new Uri(path + "?" + routeValues
                .Select(x => string.Format("{0}={1}", ToSnakeCase(x.Key), x.Value))
                .Aggregate((x, y) => x + "&" + y));
        }

        private string ToSnakeCase(string camelCase)
        {
            return string.Concat(camelCase.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }
    }
}
