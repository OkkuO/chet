using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using chet.Configurations;
using chet.Models;
using Microsoft.EntityFrameworkCore;

namespace chet.Data
{
    public class ChatsDbContext : DbContext
    {
        public ChatsDbContext(DbContextOptions<ChatsDbContext> options)
            : base(options)
        {
                
        }
        public DbSet <Chats> chats{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ChatsConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        public static implicit operator ChatsDbContext(GunDbContext v)
        {
            throw new NotImplementedException();
        }
    }
}