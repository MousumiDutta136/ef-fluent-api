using System;
using EfFluentApi.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace EfFluentApi.ConsoleApp
{
    class Program
    {
        private static IConfigurationRoot _configurationRoot;

        static void Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();

            Console.WriteLine("---------- Console App Started ----------");

            var connectionString = _configurationRoot.GetConnectionString("EfFluentApi");
            var options = new DbContextOptionsBuilder<EfFluentApiContext>()
                .UseSqlServer(connectionString)
                .Options;

            using var context = new EfFluentApiContext(options);

            CreateDb(context);

            Populate(context);

            Print(context);

            Console.WriteLine("---------- Console App Ended ----------");
        }

        private static void Print(EfFluentApiContext context)
        {
            Console.WriteLine("Printing database data...");
            foreach (var category in context.Categories)
            {
                Console.WriteLine(category);
                foreach (var product in category.Products)
                {
                    Console.WriteLine(product);
                }
            }
        }

        private static void CreateDb(EfFluentApiContext context)
        {
            Console.WriteLine("Deleting database...");
            context.Database.EnsureDeleted();
            Console.WriteLine("Creating database...");
            context.Database.EnsureCreated();
        }

        private static void Populate(EfFluentApiContext context)
        {
            Console.WriteLine("Populating database...");
            var categories = new[]
            {
                new Category{ Name = "Clothing", Products = new []
                {
                    new Product { Name = "Blue Shirt", Description = "Blue Shirt in S, M and L sizes"},
                    new Product { Name = "Red Shirt", Description = "Red Shirt in S, M and L sizes"},
                    new Product { Name = "Track Pants", Description = "Track Pants  in S, M and L sizes"}
                }},
                new Category{ Name = "Shoes", Products = new []
                {
                    new Product { Name = "Loafers", Description = "Loafers in all sizes"},
                    new Product { Name = "Sneakers", Description = "Sneakers in all sizes"}
                }}
            };
            context.Categories.AddRange(categories);
            context.SaveChanges();
        }

        static IHostBuilder CreateHostBuilder(string[] args) => Host
            .CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, configuration) =>
            {
                configuration.Sources.Clear();
                configuration.AddJsonFile("appsettings.json");
                _configurationRoot = configuration.Build();
            });
    }
}
