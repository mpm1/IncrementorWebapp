using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebIncrementor.Models
{
    public class IncrementorDBContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Incrementor> Incrementors { get; set; }

        public bool CanUseTransactions { get; private set; }

        public IncrementorDBContext() : base()
        {
            CanUseTransactions = false;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbType = (Startup.DatabaseType ?? "memory").ToLower();

            switch (dbType)
            {
                case "memory":
                    optionsBuilder.UseInMemoryDatabase("IncrementorDb");
                    break;

                case "sqlserver":
                    optionsBuilder.UseSqlServer(Startup.ConnectionString);
                    CanUseTransactions = true;
                    break;

                default:
                    throw new ArgumentException(string.Format("Invalid database type: {0}", dbType));
            }
        }
    }
}
