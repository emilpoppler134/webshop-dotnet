using System;
using Microsoft.AspNetCore.Mvc;
using Webshop.Models;

namespace Webshop.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductContext _context;

        public ProductController(ProductContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null || !int.TryParse(id, out int productId))
            {
                return BadRequest();
            }

            Product? product = await _context.Products.FindAsync(productId);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}

