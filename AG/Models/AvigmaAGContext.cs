using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AG.Models
{
    public class AvigmaAGContext : DbContext
    {
        public AvigmaAGContext(DbContextOptions<AvigmaAGContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}