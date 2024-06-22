using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PaperTrader.Models;

namespace PaperTrader.Data
{
    public class PaperTraderContext : DbContext
    {
        public PaperTraderContext(DbContextOptions<PaperTraderContext> options)
            : base(options)
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<Portfolio> Portfolio { get; set; }
        public DbSet<Stock> Stock { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Portfolio>()
                .HasOne(p => p.User)
                .WithMany(u => u.Portfolios)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Portfolio>()
                .HasMany(p => p.Stocks)
                .WithOne(s => s.Portfolio)
                .HasForeignKey(s => s.PortfolioId);
        }
    }
}
