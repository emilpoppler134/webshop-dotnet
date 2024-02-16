using Microsoft.EntityFrameworkCore;
using Webshop.Models;

public class ProductContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public string DbPath { get; }

    public ProductContext()
    {
		var folder = Environment.SpecialFolder.LocalApplicationData;
		var path = Environment.GetFolderPath(folder);
		DbPath = Path.Join(path, "webshop.db");
    }

	public void Initialize()
	{
		Database.EnsureCreated();

		if (Products != null && !Products.Any())
		{
			SeedData();
		}
	}

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

    private void SeedData()
    {
        Products.AddRange(
            new Product { Id = 1, Price = 5000 },
            new Product { Id = 2, Price = 5000 },
            new Product { Id = 3, Price = 5000 },
            new Product { Id = 4, Price = 5000 }
        );

        SaveChanges();
    }
}