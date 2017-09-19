using System;
using Microsoft.EntityFrameworkCore;

namespace exampleWebAPI.Context
{
    public class MetingContext : DbContext
    {
        public DbSet<exampleWebAPI.Models.Meting> MetingItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string ipAddress = "145.74.164.65";
            Console.WriteLine("ip: " + ipAddress);
            optionsBuilder.UseMySql(@"Server="+ ipAddress +";port=32768;database=metingen;uid=root;pwd=root;");
        }
    }
}