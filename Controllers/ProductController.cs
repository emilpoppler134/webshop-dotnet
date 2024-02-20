using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Webshop.Models;

namespace Webshop.Controllers
{
    public class ProductController : Controller
    {
        private readonly WebshopContext _context;

        public ProductController(WebshopContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null || !int.TryParse(id, out int productId))
            {
                return BadRequest();
            }

            var response = await (
                from product in _context.Products
                join stock in _context.Stocks on product.Id equals stock.ProductId
                where product.Id == productId
                select new ProductExtended
                {
                    Id = product.Id,
                    Name = product.Name,
                    Image = product.Image,
                    Stock = _context.Stocks.Where(s => s.ProductId == product.Id).ToList()
                }
            )
            .FirstOrDefaultAsync();

            if (response == null)
            {
                return NotFound();
            }

            ProductExtended extendedProduct = response;

            return View(extendedProduct);
        }
    }
}

