# MvcSample -零度分享第12期MVC&EF&VIEW练习
### 搭框架
- 新建项目
- 安装EF
- 配置数据库连接字符串
···
  <connectionStrings>
    <add name="myConn" connectionString="server=.;database=MvcSample;uid=sasa;pwd=pass@word123;" providerName="System.Data.SqlClient"/>
  </connectionStrings>
···
- 添加Ef文件夹，写入Book类，及数据上下文类MvcDbContext（继承自DbContext）
···
public class Book
    {
        public int ID { get; set; }
        [MaxLength(20)]
        public string Name { get; set; }
        public double Price { get; set; }
    }
···
···
 public class MvcDbContext:DbContext
    {
        public MvcDbContext() : base("myConn") { }//传入Web.Config中连接字串名字
        public DbSet<Book> Books { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)//移除约定 让迁移的表名为单数形式，默认是以Books命名数据表
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
···

- 数据库迁移的几个命令
1.Enable-Migrations  自动生成数据库迁移配置文件Configuration.cs，设置允许自动迁移AutomaticMigrationsEnabled = true;
2.Update-Database -force 强制更新数据库
** 小提示 **
* Enable-Migrations仅执行一次 *
* Update-Database不会删除原数据表中的记录 *
* Update-Database -force在缩小字段属性时用到强制转换参数 *



0：25：45
### MVC
#### 视图View
- 当视图中输入@会出现智能提示，这是因为在视图文件夹中的Web.config中添加了相应的引用。
· <add namespace="System.Web.Mvc" /> ·
- 视图中样式的引用其实是在App_Start/BundleConfig.cs中定义好的虚拟引用。
- 视图中引用分部视图@Html.Partial("_Menu")
- 视图启动时首先加载_ViewStart.cshtml中内容，不使用模板页可设置Layout = null;

0：50：00
#### 扩展View
···
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
···


### 中英文版本
通过资源、路由、过滤器、更改线程语言文化实现。生成后自动产生语言包，可删除不用
访问形式http://localhost:54325/zh-cn/book/create  http://localhost:54325/en-cu/book/create
- 新建资源 属性都public 名称：BookName 值：书名   复制并改名Resources.en-us.resx名称：BookName 值：Book is Name 

- 类字段约束为[Display(ResourceType = typeof(Resources), Name = "BookName")]
- 路由配置为
···
routes.MapRoute(
                name: "Default",
                url: "{lg}/{controller}/{action}/{id}",
                defaults: new {lg="zh-cn", controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
···
- 过滤器
···
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
···
- FilterConfig.cs中添加全局过滤器
· filters.Add(new MyActionFilter());//添加自定义全局过滤器 ·

1:27:30
### 添加图书并显示
···
namespace MvcSample.Controllers
{
    public class BookController : Controller
    {
        // GET: Book
        public ActionResult Index()
        {
            MvcDbContext db = new MvcDbContext();
            return View(db.Books);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Book book)
        {
            MvcDbContext db = new MvcDbContext();
            //db.Entry(book).State = System.Data.Entity.EntityState.Added;//状态跟踪
            db.Books.Add(book);
            db.SaveChanges();
            return RedirectToAction(nameof(Index));//nameof(Index) 当Index控制器重命名时跟着重命名
        }
    }
}
···
** 查看数据库连接  sp_who **
