﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Stripe;

namespace Webshop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddJsonFile("appsettings.local.json")
                .Build();

            // Configure Stripe
            StripeConfiguration.ApiKey = configuration.GetSection("Stripe")["SecretKey"];

            builder.Services.AddControllersWithViews();

            builder.Services.AddControllers().AddNewtonsoftJson();

            builder.Services.AddScoped<WebshopContext>(provider =>
            {
                return new WebshopContext();
            });

            var server = builder.Build();

            using (var serviceScope = server.Services.CreateScope())
            {
                var serviceProvider = serviceScope.ServiceProvider;
                var webshopContext = serviceProvider.GetRequiredService<WebshopContext>();
                webshopContext.Initialize();
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
                name: "search",
                pattern: "search",
                defaults: new { controller = "Home", action = "Search" });

            server.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}",
                defaults: new { controller = "Home", action = "Index" });

            server.Run();
        }
    }
}
