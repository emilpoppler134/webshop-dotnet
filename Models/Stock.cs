using System;

namespace Webshop.Models
{
    public class Stock
    {
        public int Id { get; set; }
        public int Price { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
    }
}