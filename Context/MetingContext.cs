using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.AzureAppServices;

namespace exampleWebAPI.Context
{
    public class MetingContext : DbContext
    {
        public DbSet<exampleWebAPI.Models.Meting> MetingItems { get; set; }
        public DbSet<exampleWebAPI.Models.JorgToken> JorgTokenItems { get; set; }
        
        public static IConfigurationRoot Configuration { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
            
            Console.WriteLine("Home Directory:" + Directory.GetCurrentDirectory());
            Console.Write("Database String:");
            Console.WriteLine($"{Configuration.GetConnectionString("MetingDatabase")}");
            
            
            
            optionsBuilder.UseMySql($"{Configuration.GetConnectionString("MetingDatabase")}");
        }
    }
}