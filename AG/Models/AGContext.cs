using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AG.Models
{
    public class AGContext : DbContext
    {
        public AGContext(DbContextOptions<AGContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserDetails>().ToTable("user_details");
        }

        public DbSet<UserDetails> UserDetails { get; set; }

    }
}
