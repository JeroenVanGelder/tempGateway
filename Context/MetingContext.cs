using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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
            
            optionsBuilder.UseMySql(Configuration.GetConnectionString("MetingDatase"));
        }
    }
}