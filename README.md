# MvcSample -零度分享第12期MVC&EF&VIEW练习
### 搭框架
- 新建项目
- 安装EF
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
        public MvcDbContext() : base("myConn") { }//传入Configuration中连接字串名字
        public DbSet<Book> Books { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)//移除约定 让迁移的表名为单数形式，默认是以Books命名数据表
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
···
- 配置数据库连接字符串
···
  <connectionStrings>
    <add name="myConn" connectionString="server=.;database=MvcSample;uid=ZKEACMS;pwd=ZKEACMS;" providerName="System.Data.SqlClient"/>
  </connectionStrings>
···
- 数据库迁移的几个命令
1.Enable-Migrations  自动生成数据库迁移配置文件Configuration.cs，设置允许自动迁移AutomaticMigrationsEnabled = true;
2.Update-Database -force 强制更新数据库
**小提示**
**Enable-Migrations仅执行一次
**Update-Database不会删除原数据表中的记录
**Update-Database -force在缩小字段属性时用到强制转换参数



0：25：45
### MVC

