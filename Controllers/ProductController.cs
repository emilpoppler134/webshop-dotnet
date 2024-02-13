using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Webshop.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Webshop.Controllers
{
    public class ProductController : Controller
    {
        // GET: /<controller>/
        public IActionResult Details(String id)
        {
            Product product = new(id, "Emil Poppler");

            return View(product);
        }
    }
}

