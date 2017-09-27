using MvcSample.Ef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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