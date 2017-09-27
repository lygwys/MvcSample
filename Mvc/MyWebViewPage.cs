using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcSample.Mvc
{/// <summary>
/// view扩展，需要在Views/Web.config中更改默认pages pageBaseType="System.Web.Mvc.WebViewPage"为 "MvcSample.Mvc.MyWebViewPage" >  前台@可以提示了@Helper.HelloWerld()
/// </summary>
/// <typeparam name="TModel"></typeparam>
    public abstract class MyWebViewPage<TModel> : WebViewPage<TModel>
    {
        public MyWebViewPage()
        {
            this.Helper = new MyHelper();
        }
        public MyHelper Helper { get; set; }
    }

    public class MyHelper
    {
        public MvcHtmlString HelloWerld()
        {
            return new MvcHtmlString("hello.world!");//自定义任意方法
        }
    }
}