using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebApiRestfulBestPractice.Paging
{
    /// <summary>
    /// 產生有分頁資訊的 Result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedResult<T>
    {
        private IEnumerable<T> pagedRows;
        private PageDataBuilder dataBuilder;

        /// <summary>
        /// 用 totalRows 取得總筆數,做分頁後再附加分頁資訊
        /// </summary>
        /// <param name="totalRows">全部的資料列</param>
        /// <param name="pageNo">需求的頁碼</param>
        /// <param name="pageSize">每頁的筆數</param>
        /// <param name="requestPath">服務路徑 ex:Request.RequestUri.GetLeftPart(UriPartial.Path), 沒傳就不產生 Link</param>
        /// <param name="routeValues">Link 的 Url 參數, 匿名物件</param>
        public PagedResult(IEnumerable<T> totalRows,
            int pageNo, 
            int pageSize,
            string requestPath = null,
            object routeValues = null)
        {
            this.dataBuilder = new PageDataBuilder(pageNo, pageSize, totalRows.Count(), requestPath, routeValues);
            //
            var skipAmount = pageSize * (pageNo - 1);
            this.pagedRows = totalRows.Skip(skipAmount).Take(pageSize);
        }

        /// <summary>
        /// 使用者已分好頁,只附加分頁資訊
        /// </summary>
        /// <param name="requestPath">服務路徑 ex:Request.RequestUri.GetLeftPart(UriPartial.Path)</param>
        /// <param name="routeValues">額外的 Url 參數</param>
        /// <param name="pageRows">要回傳的單頁資料</param>
        /// <param name="pageNo">需求的頁碼</param>
        /// <param name="pageSize">每頁的筆數</param>
        /// <param name="totalRecordCount">資料總筆數</param>
        /// <param name="requestPath">服務路徑 ex:Request.RequestUri.GetLeftPart(UriPartial.Path), 沒傳就不產生 Link</param>
        /// <param name="routeValues">Link 的 Url 參數, 匿名物件</param>
        public PagedResult(IEnumerable<T> pageRows,
            int pageNo,
            int pageSize,
            int totalRecordCount,
            string requestPath = null,
            object routeValues = null)
        {
            this.dataBuilder = new PageDataBuilder(pageNo, pageSize, totalRecordCount, requestPath, routeValues);
            this.pagedRows = pageRows;
        }

        public ResultObject<T> CreateResult()
        {
            var result = new ResultObject<T>
            {
                Data = pagedRows,
                Paging = new Paging
                {
                    PageNo = dataBuilder.PageNo,
                    PageCount = dataBuilder.PageCount,
                    PageSize = dataBuilder.PageSize,
                    TotalRecordCount = dataBuilder.TotalRecordCount
                }
            };

            if(dataBuilder.HasLink)
            {
                result.Paging.FirstPage = dataBuilder.FirstPage;
                result.Paging.LastPage = dataBuilder.LastPage;
                result.Paging.PreviousPage = dataBuilder.PreviousPage;
                result.Paging.NextPage = dataBuilder.NextPage;
            }

            return result;
        }

        /// <summary>
        /// 單取得分頁資料
        /// </summary>
        /// <returns></returns>
        public PageDataBuilder GetPageDataBuilder()
        {
            return dataBuilder;
        }
    }

    public class ResultObject<T>
    {
        public ResultObject()
        {
            Paging = new Paging();
        }

        public IEnumerable<T> Data { get; set; }
        //
        public Paging Paging { get; set; }
    }

    public class Paging
    {
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
        public int TotalRecordCount { get; set; }
        public Uri FirstPage { get; set; }
        public Uri LastPage { get; set; }
        public Uri PreviousPage { get; set; }
        public Uri NextPage { get; set; }
    }
}
