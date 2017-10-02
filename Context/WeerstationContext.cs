using System;
using Microsoft.EntityFrameworkCore;

namespace exampleWebAPI.Context
{
    public class WeerstationContext : DbContext
    {
        public DbSet<Models.Weerstation> Weerstation { get; set; }
        public DbSet<Models.Meting> Meting { get; set; }
        public DbSet<Models.Token> Token { get; set; }
        public DbSet<Models.User> User { get; set; }
        public DbSet<Models.MyCookie> MyCookies{ get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string ipAddress = "localhost";
            Console.WriteLine("ip: " + ipAddress);
            optionsBuilder.UseMySql(@"Server=" + ipAddress + ";database=Weerstation;uid=root;");
        }
        /*
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.User>()
                .HasKey(c => new { c.Email});
        }
        */
    }
}