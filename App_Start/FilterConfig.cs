using MvcSample.Controllers;
using System.Web;
using System.Web.Mvc;

namespace MvcSample
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new MyActionFilter());//添加自定义全局过滤器
            filters.Add(new HandleErrorAttribute());
        }
    }
}
