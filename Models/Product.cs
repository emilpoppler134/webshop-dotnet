namespace Webshop.Models
{
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public Product(String id, String name)
        {
            Id = id;
            Name = name;
        }
    }
}