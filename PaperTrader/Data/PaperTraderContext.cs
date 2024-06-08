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
        public PaperTraderContext (DbContextOptions<PaperTraderContext> options)
            : base(options)
        {
        }

        public DbSet<PaperTrader.Models.User> User { get; set; } = default!;
    }
}
