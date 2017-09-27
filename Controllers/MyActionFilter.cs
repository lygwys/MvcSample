using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace MvcSample.Controllers
{
    public class MyActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var routeData = filterContext.RouteData;//获取路由数据,打断点可查看数据
            string lg =  filterContext.RouteData.Values["lg"].ToString();//获取lg值
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(lg);//更改线程中的语言文化
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lg);//更改UI线程中的语言文化
        }
    }
}