using Microsoft.EntityFrameworkCore;
using Webshop.Models;

public class ProductContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Stock> Stocks { get; set; }

	public void Initialize()
	{
		Database.EnsureCreated();

		if (!Products.Any())
		{
			SeedProductData();
		}

		if (!Stocks.Any())
		{
			SeedStockData();
		}
	}

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source=Webshop.db");

    private void SeedProductData()
    {
        Products.AddRange(
            new Product { Name = "Apostle Shirt", Image = "2e123620-5c1a-4516-9fcd-2e7fef7860e2" },
            new Product { Name = "T-Bone Hoodie", Image = "5b2dae83-5460-492d-af72-37b365a06dfa" },
            new Product { Name = "Evisu hoodie", Image = "082d8b95-93a04a-d4d4-2ecbf433d6" }
        );

        SaveChanges();
    }

    private void SeedStockData()
    {
        Stocks.AddRange(
            new Stock { Price = 300, Size = "S", Quantity = 2, ProductId = 1 },
            new Stock { Price = 350, Size = "M", Quantity = 4, ProductId = 1 },
            new Stock { Price = 400, Size = "L", Quantity = 1, ProductId = 1 },
            new Stock { Price = 500, Size = "S", Quantity = 0, ProductId = 2 },
            new Stock { Price = 600, Size = "M", Quantity = 0, ProductId = 2 },
            new Stock { Price = 700, Size = "L", Quantity = 1, ProductId = 2 },
            new Stock { Price = 1000, Size = "S", Quantity = 5, ProductId = 3 },
            new Stock { Price = 2000, Size = "M", Quantity = 5, ProductId = 3 },
            new Stock { Price = 3000, Size = "L", Quantity = 0, ProductId = 3 }
        );

        SaveChanges();
    }
}