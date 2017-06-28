# Code for ASP.NET Web API RESTFul Best Practice
ASP.NET Web API Best Practice 專案整合 - 客製篇

## 簡介

在 ASP.NET Web API 2 基礎下，建立整合完備的應用程式專案範本。

* 整合套件或自行開發來提供 `RESTFul API 最佳實踐` 中的建議功能。
* 配合 `開發規範` 提供一致有效率的開發方式及程式品質。

這裡提供一些自行開發的程式範例，以達到對 RESTFul 最佳實踐的支援。

## 系列文章

* [RESTFul API 最佳實踐](http://yingclin.github.io/restful-api-best-practice.html)
* [ASP.NET Web API 最佳實踐專案整合 - 自帶篇](http://yingclin.github.io/asp-net-web-api-restful-best-practice-1.html)
* [ASP.NET Web API 最佳實踐專案整合 - 套件篇](http://yingclin.github.io/asp-net-web-api-restful-best-practice-2.html)
* [ASP.NET Web API 最佳實踐專案整合 - 客製篇](http://yingclin.github.io/asp-net-web-api-restful-best-practice-3.html)

## 版本資訊

### 以 Owin Middleware 實作 Versioning Header

版本資訊，可用自動取得的 Assembly File Version 來表示，並保存起來，避免一直重覆讀取。進階上，可以配合參數，傳入指定的版本資訊或類型，提供更彈性的功能。

取到的是哪一個 Assembly 會影響拿到的版本內容。Assembly.GetExecutingAssembly()　是取 Middleware 實作的這個 Assembly。如果 Middleware 實作在另外的專案中，專案中有提供自動找到繼承 ApiController 的 Assembly 方法。

在啟動類別中啟用
```
app.UseVersioningMiddleware();
```

## 套用 snake_case

### 網址參數轉換  

自行實作 ActionSelector，原理就是對 QueryString 做轉換和重寫。 

修改 `WebApiConfig` 以啟用功能
```
config.Services.Replace(typeof(IHttpActionSelector), new SnakeCaseActionSelector());
```

## 分頁

自己實作產生 Link header、Paging header 及 Response 分頁資訊 (JSON Envelope) 的工具類別

用法範例：

產生分頁 header (X-Paging-*)
```
// 建立不產生連結的分頁資訊
var dataBuilder = new PageDataBuilder(1, 10, 1000);

// 建立 HttpResponseMessage
var responseMessage = Request.CreateResponse(HttpStatusCode.OK);

// 加入分頁資訊 header 到 responseMessage
new PagingHeader(responseMessage)
    .AddPageNo(dataBuilder.PageNo)
    .AddPageSize(dataBuilder.PageSize)
    .AddPageCount(dataBuilder.PageCount)
    .AddTotalRecordCount(dataBuilder.TotalRecordCount);

return ResponseMessage(responseMessage);
```

產生分頁 header 及 Link header
```
// 建立產生連結的分頁資訊 (有 requestPath 就會產生 Link)
var dataBuilder = new PageDataBuilder(1, 10, 1000, requestPath);

// 建立 HttpResponseMessage
var responseMessage = Request.CreateResponse(HttpStatusCode.OK);

// 加入分頁資訊 header 到 responseMessage
new PagingHeader(responseMessage)
    .AddPageNo(dataBuilder.PageNo)
    .AddPageSize(dataBuilder.PageSize)
    .AddPageCount(dataBuilder.PageCount)
    .AddTotalRecordCount(dataBuilder.TotalRecordCount)
    .AddLink(dataBuilder);

return ResponseMessage(responseMessage);
```

產生分頁結果
```
// 建立分頁結果
var pagedResult = new PagedResult<Member>(members, pageNo, pageSize, requestPath);
// 有需要，可取得分頁資訊，做為輸出 header 用
var dataBuilder = pagedResult.GetPageDataBuilder();
// 在 Response 中加入 Result
var responseMessage = Request.CreateResponse(HttpStatusCode.OK, pagedResult.CreateResult());

return ResponseMessage(responseMessage);

```
輸出
```
{
  "data": [
    {
      "num": 1,
      "name": "MEM1"
    },
    {
      "num": 2,
      "name": "MEM2"
    }
  ],
  "paging": {
    "page_no": 1,
    "page_size": 2,
    "page_count": 3,
    "total_record_count": 5,
    "first_page": "https://www.shop.com/api/members?page_no=1&page_size=2",
    "last_page": "https://www.shop.com/api/members?page_no=3&page_size=2",
    "next_page": "https://www.shop.com/api/members?page_no=2&page_size=2"
  }
}
```