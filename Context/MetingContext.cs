using System;
using Microsoft.EntityFrameworkCore;

namespace exampleWebAPI.Context
{
    public class MetingContext : DbContext
    {
        public DbSet<exampleWebAPI.Models.Meting> MetingItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string ipAddress = "localhost";
            Console.WriteLine("ip: " + ipAddress);
            optionsBuilder.UseMySql(@"Server="+ ipAddress +";database=metingen;uid=root;");
        }
    }
}