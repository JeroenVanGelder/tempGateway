using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; 


namespace exampleWebAPI.Context
{
    public class WeerstationContext : DbContext
    {
        public DbSet<Models.Weerstation> Weerstation { get; set; }
        public DbSet<Models.Meting> Meting { get; set; }
        public DbSet<Models.Token> Token { get; set; }
        public DbSet<Models.User> User { get; set; }
        public DbSet<Models.MyCookie> MyCookies{ get; set; }

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