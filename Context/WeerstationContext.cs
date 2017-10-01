using System;
using Microsoft.EntityFrameworkCore;

namespace exampleWebAPI.Context
{
    public class MetingContext : DbContext
    {
        public DbSet<Models.Meting> Meting { get; set; }
        public DbSet<Models.Token> Token { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string ipAddress = "localhost";
            Console.WriteLine("ip: " + ipAddress);
            optionsBuilder.UseMySql(@"Server=" + ipAddress + ";database=Weerstation;uid=root;");
        }
    }
}