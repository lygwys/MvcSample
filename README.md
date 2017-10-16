# MvcSample -零度分享第12期MVC&EF&VIEW练习
### 搭框架
- 新建项目
- 安装EF
- 配置数据库连接字符串
-
      <connectionStrings>
    <add name="myConn" connectionString="server=.;database=MvcSample;uid=sasa;pwd=pass@word123;" providerName="System.Data.SqlClient"/>
      </connectionStrings>

- 添加Ef文件夹，写入Book类，及数据上下文类MvcDbContext（继承自DbContext）
-
    public class Book
    {
	    public int ID { get; set; }
	    [MaxLength(20)]
	    public string Name { get; set; }
	    public double Price { get; set; }
    }

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



### 中英文版本
通过资源、路由、过滤器、更改线程语言文化实现。生成后自动产生语言包，可删除不用
访问形式http://localhost:54325/zh-cn/book/create  http://localhost:54325/en-cu/book/create
- 新建资源 属性都public 名称：BookName 值：书名   复制并改名Resources.en-us.resx名称：BookName 值：Book is Name 

- 类字段约束为[Display(ResourceType = typeof(Resources), Name = "BookName")]
- 路由配置为
-
    routes.MapRoute(
    name: "Default",
    url: "{lg}/{controller}/{action}/{id}",
    defaults: new {lg="zh-cn", controller = "Home", action = "Index", id = UrlParameter.Optional }
    );

- 过滤器  
- 

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

- FilterConfig.cs中添加全局过滤器  
    `filters.Add(new MyActionFilter());//添加自定义全局过滤器 `

1:27:30
### 添加图书并显示
-  
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

** 查看数据库连接  sp_who **






# EfDemo -零度分享第14期(EF批量数据 死锁检验 事务 并发）练习
数据库分区分表、复制、AlwaysOn高可用性
0:21:45
### 搭框架
- 新建EfDemo控制台项目
- 安装EF框架
- 添加数据库连接字符串
- 新建/Models/Book类
-
    namespace EfDemo.Models
    {
	    public class Book
	    {
		    public int ID { get; set; }
		    public string Name { get; set; }
		    public decimal Price { get; set; }
		    public DateTime CreateDate { get; set; }
	    }
    }

- 新建映射类/Maping/BookMap
-  
    namespace EfDemo.Mapping
    {
	    public class BookMap : EntityTypeConfiguration<Book>
	    {
		   	 public BookMap()
		    {
			    this.Property(b => b.Name).HasMaxLength(20);
			    this.Property(b => b.Price).HasColumnType("money");
		    }
	    }
    }

- 新建数据库上下文类
-  
    namespace EfDemo.Models
    {
	    public class DemoDbContext : DbContext
	    {
		    public DemoDbContext() : base("demo") { }
		    protected override void OnModelCreating(DbModelBuilder modelBuilder)
		    {
			    modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();//移除约定 让迁移的表名为单数形式，默认是以Books命名数据表
			    modelBuilder.Configurations.AddFromAssembly(Assembly.GetExecutingAssembly());//扫描当前配置的东西全加载进来，Mapping就会生效
			    base.OnModelCreating(modelBuilder);
		    }
		    public DbSet<Book> Books { get; set; }
	    }
    }

- 允许数据迁移
· >Enable-Migrations ·
 Configuration.cs  `AutomaticMigrationsEnabled = true;`
- 数据迁移  
 `>Updata-database`

0:39:30
### EF扩展提高性能
- 安装EntityFramework.Extended
- 新建类EfExtendedDemo/EfExtendedSample.cs
-
    using EntityFramework.Extensions;
    using EfDemo.Models;
    using EntityFramework.Caching;
    using EntityFramework;
    
    namespace EfDemo.EfExtendedDemo
    {
	    class EfExtendedSample
	    {
	    	public void BatchDelete()//批量删除
		    {
			    DemoDbContext db = new DemoDbContext();
			    db.Books.Where(b => b.Price <= 100).Delete();
			    db.SaveChanges();
		    }
		    
		    public void BatchUpdate()//批量更新
		    {
			    DemoDbContext db = new DemoDbContext();
			    db.Books.Where(b => b.Price <= 100).Update(bc => new Book { Price = bc.Price + 10 });
			    db.SaveChanges();
		    }
		    
		    public void BatchQuery()//批量查询
		    {
			    DemoDbContext db = new DemoDbContext();
			    //var books = db.Books.Where(b => b.Price >= 100).ToList();
			    //var books2 = db.Books.Where(b => b.Price <= 100).ToList();
			    var q1 = db.Books.Where(b => b.Price >= 100).Future();//加上 .Future()进行标记就可以在一个查询连接中执行多个查询
			    var q2 = db.Books.Where(b => b.Price <= 100).Future();
			    var books = q1.ToList();
			    var books2 = q2.ToList();
		    }
		    
		    public void CacheQueryResult()//使用缓存
		    {
			    DemoDbContext db = new DemoDbContext();
			    var q1 = db.Books.Where(b => b.Price >= 100).FromCache();
			    var q2 = db.Books.Where(b => b.Price >= 100).FromCache(CachePolicy.WithDurationExpiration(TimeSpan.FromMinutes(10)));//10钟后过期
			    
			    var q3 = db.Books.Where(b => b.Price >= 100).FromCache(tags:new[] { "book111","oldbook"});//给缓存打N个标记
			    CacheManager.Current.Expire("book111");//让标记的缓存过期
		    
		       // Locator.Current.Register<ICacheProvider>(() =>new RedisCache());//配置一下Redis即可实现分布式缓存
		    }
	    }
    }
    
···
0:59:40
### 事务
1:33:05
### 文件事务
要下载一个老外写的文件底层操作类TransactedFile.cs  
` TransactedFile.Delete(@"C:\1.jpg")`  


1:40:44
### EF方面事务

    using EfDemo.Models;
    using System.Data.SqlClient;
    using System.Transactions;
    /// <summary>
    /// EF事务示例 引用System.Transactions;
    /// </summary>
    namespace EfDemo.EfTransactionDemo
    {
	    public class EfTranSample
	    {
		    public void TransactionScoptTest()//常规的用法
		    {
		    	using (var tran = new TransactionScope())
			    {
				    DemoDbContext db1 = new DemoDbContext();
				    db1.Books.Find(1).Name = "C+++";
				    DemoDbContext db2 = new DemoDbContext();
				    db2.Books.Find(2).Name = "C++++";
				    db1.SaveChanges();
				    db2.SaveChanges();
				    tran.Complete();
			    }
		    }
		    
		    public void Ef6TransactionTest()//EF6将语句嵌入事务
		    {
			    DemoDbContext db = new DemoDbContext();
			    using (var tran = db.Database.BeginTransaction())
			    {
			    	try
				    {
					    string sql = "";
					    db.Database.ExecuteSqlCommand(sql);
					    db.Books.Find(1).Name = "JJJ";
					    db.SaveChanges();
				    }
				    catch 
				    {
				   	 	tran.Rollback();
				    }
			    }
		    }
		    
		    public void ExistTransactionTest()//将DbContext嵌入事务
		    {
			    using (var conn = new SqlConnection("connString"))//using System.Data.SqlClient;
			    {
				    conn.Open();
				    using (var sqlTran = conn.BeginTransaction())
					    {
					    	try
					    {
						    SqlCommand command = new SqlCommand();
						    command.Connection = conn;
						    command.Transaction = sqlTran;
						    command.CommandText = "sql";
						    command.ExecuteNonQuery();
						    DemoDbContext db = new DemoDbContext();
						    db.Database.UseTransaction(sqlTran);
						    db.Books.Find(1).Price = 11;
						    db.SaveChanges();
						    sqlTran.Commit();
					    }
					    catch 
					    {
					    	sqlTran.Rollback();   
					    }
				    }
			    }
		    }
	    }
    }
···
1:54:50
### EF异步
` db2.SaveChangesAsync(); `

1:55:30 
### EF并发处理
-  
	{
		public class ConcurrencySapmple
		{
			public void DefaultConcurrencyTest()//并发处理前提Book类加字段锁[ConcurrencyCheck]（BookMap启用字段检查this.Property(b => b.Price).IsConcurrencyToken();）或加时间戳[Timestamp]public Byte[] RowVersion { get; set; }（BookMap中启用行版本this.Property(b => b.RowVersion).IsRowVersion();）
			{
				try
				{
					DemoDbContext db1 = new DemoDbContext();
					DemoDbContext db2 = new DemoDbContext();
					
					var book1 = db1.Books.Find(1);
					var book2 = db2.Books.Find(1);
					
					book1.Price += 10;
					book1.Name = "Book1";
					
					book2.Price += 20;
					book2.Name = "Book2";
					db1.SaveChanges();
					db2.SaveChanges();		
				}
				catch (DbUpdateConcurrencyException e)//using System.Data.Entity.Infrastructure;
				{
					e.Entries.Single().Reload();//获取没保存成功的，重新装载一下，把数据库最新版本再装载一下并执行,让并行的都得以执行，不会产生覆盖
				}			
			}
		}
	}






