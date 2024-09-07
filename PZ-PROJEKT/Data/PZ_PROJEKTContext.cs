using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PZ_PROJEKT.Models;

namespace PZ_PROJEKT.Data
{
    public class PZ_PROJEKTContext : DbContext
    {
        public PZ_PROJEKTContext(DbContextOptions<PZ_PROJEKTContext> options)
            : base(options)
        {
        }

        public DbSet<PZ_PROJEKT.Models.Case> Case { get; set; } = default!;

        public DbSet<PZ_PROJEKT.Models.User> User { get; set; } = default!;

        public DbSet<PZ_PROJEKT.Models.Item> Item { get; set; }

        public DbSet<PZ_PROJEKT.Models.PaymentMethod> PaymentMethods { get; set; } = default!;

        public DbSet<PZ_PROJEKT.Models.Payment> Payments { get; set; } = default!;

        public DbSet<PZ_PROJEKT.Models.CaseHistory> CaseHistory { get; set; } = default!;

        public DbSet<PZ_PROJEKT.Models.Inventory> Inventory { get; set; } = default!;
        public DbSet<PZ_PROJEKT.Models.ItemTransaction> ItemTransactions { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
