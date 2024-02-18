using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Webshop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<ProductContext>(provider =>
            {
                return new ProductContext();
            });

            var server = builder.Build();

            using (var serviceScope = server.Services.CreateScope())
            {
                var serviceProvider = serviceScope.ServiceProvider;
                var productContext = serviceProvider.GetRequiredService<ProductContext>();
                productContext.Initialize();
            }

            if (!server.Environment.IsDevelopment())
            {
                server.UseExceptionHandler("/Home/Error");
                server.UseHsts();
            }

            server.UseHttpsRedirection();
            server.UseStaticFiles();
            server.UseRouting();
            server.UseAuthorization();

            server.MapControllerRoute(
                name: "product",
                pattern: "products/{id}",
                defaults: new { controller = "Product", action = "Details" });

            server.MapControllerRoute(
                name: "cart",
                pattern: "cart",
                defaults: new { controller = "Home", action = "Cart" });

            server.MapControllerRoute(
                name: "checkout",
                pattern: "checkout",
                defaults: new { controller = "Home", action = "Checkout" });

            server.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}",
                defaults: new { controller = "Home", action = "Index" });

            server.Run();
        }
    }
}
