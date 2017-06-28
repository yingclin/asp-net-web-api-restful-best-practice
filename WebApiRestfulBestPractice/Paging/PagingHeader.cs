using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebApiRestfulBestPractice.Paging
{
    /// <summary>
    /// 在 Response 中設定分頁相關 Header
    /// </summary>
    public class PagingHeader
    {
        private const string PageNoHeaderName = "X-Paging-PageNo";
        private const string PageSizeHeaderName = "X-Paging-PageSize";
        private const string PageCountHeaderName = "X-Paging-PageCount";
        private const string TotalRecordCountName = "X-Paging-TotalRecordCount";
        private const string LinkHeaderName = "Link";
        private const string LinkHeaderTemplate = "<{0}>; rel=\"{1}\"";
        //
        private HttpResponseMessage response;

        public PagingHeader(HttpResponseMessage responseMessage)
        {
            this.response = responseMessage;
        }

        public PagingHeader AddPageNo(int pageNo)
        {
            response.Headers.Add(PageNoHeaderName, pageNo.ToString());

            return this;
        }

        public PagingHeader AddPageSize(int pageSize)
        {
            response.Headers.Add(PageSizeHeaderName, pageSize.ToString());

            return this;
        }

        public PagingHeader AddPageCount(int pageCount)
        {
            response.Headers.Add(PageCountHeaderName, pageCount.ToString());

            return this;
        }

        public PagingHeader AddTotalRecordCount(int totalCount)
        {
            response.Headers.Add(TotalRecordCountName, totalCount.ToString());

            return this;
        }

        public PagingHeader AddLink(PageDataBuilder dataBuilder)
        {
            if(!dataBuilder.HasLink)
            {
                return this;
            }

            List<string> links = new List<string>();

            if (dataBuilder.FirstPage != null)
            {
                links.Add(string.Format(LinkHeaderTemplate, dataBuilder.FirstPage, "first"));
            }

            if (dataBuilder.PreviousPage != null)
            {
                links.Add(string.Format(LinkHeaderTemplate, dataBuilder.PreviousPage, "previous"));
            }

            if (dataBuilder.NextPage != null)
            {
                links.Add(string.Format(LinkHeaderTemplate, dataBuilder.NextPage, "next"));
            }

            if (dataBuilder.LastPage != null)
            {
                links.Add(string.Format(LinkHeaderTemplate, dataBuilder.LastPage, "last"));
            }

            // Set the page link header
            response.Headers.Add(LinkHeaderName, string.Join(", ", links));

            return this;
        }
    }
}
