using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Webshop.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stripe;

namespace Webshop.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly WebshopContext _context;
        public CheckoutController(WebshopContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
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

        public IActionResult Success()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Charge([FromBody] Payload payload)
		{
            try
            {
                var products = payload.products;
                var customerInfo = payload.customer;
                var billingAddress = customerInfo.billing;
                var shippingAddress = payload.shipping;
                var paymentInfo = payload.payment;

                string ccExp = paymentInfo.cc_exp;
                string[] expParts = ccExp.Split('/');
                string ccExpMonth = expParts[0].Trim();
                string ccExpYear = expParts[1].Trim();
                
                int amount = 99;

                // Validation
                foreach (string item in products)
                {
                    StockExtended? extendedStock = await (
                        from stock in _context.Stocks
                        join product in _context.Products on stock.ProductId equals product.Id
                        where stock.Id == int.Parse(item)
                        select new StockExtended
                        {
                            Id = stock.Id,
                            Price = stock.Price,
                            Size = stock.Size,
                            Quantity = stock.Quantity,
                            Product = _context.Products.Where(s => s.Id == stock.ProductId).First()
                        }
                    ).FirstOrDefaultAsync();

                    if (extendedStock == null)
                    {
                        return BadRequest();
                    }
                    if (extendedStock.Quantity == 0)
                    {
                        return BadRequest();
                    }

                    amount += extendedStock.Price;
                }


                // Create Stripe token
                var tokenOptions = new TokenCreateOptions
                {
                    Card = new TokenCardOptions
                    {
                        Name = paymentInfo.cc_name,
                        Number = paymentInfo.cc_number,
                        ExpMonth = ccExpMonth,
                        ExpYear = ccExpYear,
                        Cvc = paymentInfo.cc_csc,
                    },
                };
                var tokenService = new TokenService();
                var token = await tokenService.CreateAsync(tokenOptions);


                // Create Stripe customer
                var customerOptions = new CustomerCreateOptions
                {
                    Name = customerInfo.name,
                    Email = customerInfo.email,
                    Phone = customerInfo.phone,
                    Address = new AddressOptions
                    {
                        Line1 = billingAddress.line1,
                        Line2 = null,
                        PostalCode = billingAddress.postal_code,
                        City = billingAddress.city,
                        State = null,
                        Country = billingAddress.country
                    },
                    Shipping = new ShippingOptions
                    {
                        Name = shippingAddress.name,
                        Phone = shippingAddress.phone,
                        Address = new AddressOptions
                        {
                            Line1 = shippingAddress.address.line1,
                            Line2 = null,
                            PostalCode = shippingAddress.address.postal_code,
                            City = shippingAddress.address.city,
                            State = null,
                            Country = shippingAddress.address.country
                        }
                    },
                    Source = token.Id
                };
                var customerService = new CustomerService();
                var customer = await customerService.CreateAsync(customerOptions);


                // Create Stripe charge
                var chargeOptions = new ChargeCreateOptions
                {
                    Amount = amount * 100,
                    Currency = "sek",
                    Customer = customer.Id,
                    ReceiptEmail = customer.Email,
                    Shipping = new ChargeShippingOptions
                    {
                        Name = customerInfo.name,
                        Phone = customerInfo.phone,
                        Address = new AddressOptions
                        {
                            Line1 = shippingAddress.address.line1,
                            Line2 = null,
                            PostalCode = shippingAddress.address.postal_code,
                            City = shippingAddress.address.city,
                            State = null,
                            Country = shippingAddress.address.country
                        }
                    }
                };
                var chargeService = new ChargeService();
                var charge = await chargeService.CreateAsync(chargeOptions);

                foreach (string item in products)
                {
                    // Select products from stock Id       
                    var stockExtended = await (
                        from stock in _context.Stocks
                        join product in _context.Products on stock.ProductId equals product.Id
                        where stock.Id == int.Parse(item)
                        select new StockExtended
                        {
                            Id = stock.Id,
                            Price = stock.Price,
                            Size = stock.Size,
                            Quantity = stock.Quantity,
                            Product = _context.Products.FirstOrDefault(s => s.Id == stock.ProductId)
                        }
                    ).FirstOrDefaultAsync();

                    if (stockExtended == null)
                    {
                        return BadRequest();
                    }

                    // Remove one from quantity
                    stockExtended.Quantity--;
                }

                _context.SaveChanges();

                return Ok(charge);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while saving changes: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return BadRequest(ex.Message);
            }
        }
	}
}