using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Webshop.Models;
using Newtonsoft.Json;

namespace Webshop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductContext _context;

        public HomeController(ProductContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> Cart()
        {
            string? cookies = Request.Cookies["cart"];
            List<int> cart = new List<int>();

            try
            {
                if (cookies != null)
                {
                    string cartString = Uri.UnescapeDataString(cookies);
                    List<int>? convertedCart = JsonConvert.DeserializeObject<List<int>>(cartString);
                    
                    if (convertedCart != null)
                    {
                        cart = convertedCart;
                    }
                }
            }
            catch (JsonReaderException ex)
            {
                return BadRequest("Invalid format in cart cookie: " + ex.Message);
            }

            var response = await (
                from stock in _context.Stocks
                join product in _context.Products on stock.ProductId equals product.Id
                where cart.Contains(stock.Id)
                select new StockExtended
                {
                    Id = stock.Id,
                    Price = stock.Price,
                    Size = stock.Size,
                    Quantity = stock.Quantity,
                    Product = _context.Products.Where(s => s.Id == stock.ProductId).First()
                }
            ).ToListAsync();

            List<StockExtended> extendedStockList = response;

            return View(extendedStockList);
        }

        public async Task<IActionResult> Checkout()
        {
            return View();
        }
    }
}
