using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcSample.Ef
{
    public class Book
    {
        public int ID { get; set; }
        [MaxLength(20)]
        public string Name { get; set; }
        public double Price { get; set; }
        public DateTime BuyTime { get; set; }

    }
}