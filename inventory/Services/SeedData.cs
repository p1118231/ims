namespace inventory.Services;

using System;
using inventory.Data;
using inventory.Models;

public class SeedData{

    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ProductContext>();

            // Ensure database is created
            context.Database.EnsureCreated();

            // Seed Suppliers
            if (!context.Suppliers.Any())
            {
                context.Suppliers.AddRange(
                    new Supplier { Name = "GoodFit Supplies", Address = "1234 Fitness St, Wellness City", Email = "contact@goodfitsupplies.com", Phone = "123-456-7890", Products = new List<Product>() },
                    new Supplier { Name = "HealthGear Inc.", Address = "4321 Health Ave, Bodytown", Email = "info@healthgearinc.com", Phone = "234-567-8901", Products = new List<Product>() }
                );
                context.SaveChanges();
            }

            // Seed Categories
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { Name = "Fitness", Products = new List<Product>() },
                    new Category { Name = "Wellness", Products = new List<Product>() }
                );
                context.SaveChanges();
            }

            // Seed Products
            if (!context.Product.Any())
            {
                context.Product.AddRange(
                    new Product { Name = "Yoga Mat", Description = "High-quality, durable mat ideal for all types of yoga.", Quantity=463, Price = 20.99M, CategoryId = 1, SupplierId = 1, ImageUrl = "https://media.istockphoto.com/id/1907365112/photo/red-exercise-mat-object-shadow-clipping-path.webp?a=1&b=1&s=612x612&w=0&k=20&c=bjQAGNTCMNU-7EVVGdEOF4nke3fWeaVicFspQSqFj6E=" },
                    new Product { Name = "Kettlebell", Description = "Cast iron kettlebell perfect for strength training.", Quantity = 232, Price = 35.99M, CategoryId = 1, SupplierId = 2, ImageUrl = "https://media.istockphoto.com/photos/kettlebell-on-white-background-picture-id1160735799?k=6&m=1160735799&s=612x612&w=0&h=8"}
                );
                context.SaveChanges();
            }
        }
    }
}