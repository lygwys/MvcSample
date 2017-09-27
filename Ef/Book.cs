using MvcSample.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcSample.Ef
{
    public class Book
    {
        public int ID { get; set; }
        [MaxLength(20)]
        //[Display(ResourceType =typeof(Resources),Name =nameof(Book)+nameof(Name))]
        [Display(ResourceType = typeof(Resources), Name = "BookName")]
        public string Name { get; set; }
        public double Price { get; set; }
    }
}