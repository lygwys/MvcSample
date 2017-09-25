using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace MvcSample.Ef
{
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
}