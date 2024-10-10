using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using chet.Configurations;
using chet.Models;
using Microsoft.EntityFrameworkCore;

namespace chet.Data
{
    public class GunDbContext : DbContext
    {
        public GunDbContext(DbContextOptions<GunDbContext> options)
            : base(options)
            {
                
            }

        public DbSet <Gun> guns{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new GunConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}