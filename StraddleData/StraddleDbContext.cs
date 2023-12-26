using Microsoft.EntityFrameworkCore;
using StraddleData.Configurations.Partners;
using StraddleData.Configurations.Wallets;
using StraddleData.Models.Partners;
using StraddleData.Models.Wallets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleData
{
    public partial class StraddleDbContext : DbContext
    {
        public StraddleDbContext(DbContextOptions<StraddleDbContext> options)
            : base(options)
        {
        }

        //Wallets
        public virtual DbSet<WalletAccount> WalletAccounts { get; set; }
        public virtual DbSet<WalletCustomer> WalletCustomers { get; set; }
        public virtual DbSet<WalletTransaction> WalletTransactions { get; set; }

        //Partners
        public virtual DbSet<ApiClient> ApiClients { get; set; }

        //Regenerate Models and DBContext using CodeFirst From Database
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Wallets
            modelBuilder.ApplyConfiguration(new WalletAccountConfiguration());
            modelBuilder.ApplyConfiguration(new WalletCustomerConfiguration());
            modelBuilder.ApplyConfiguration(new WalletTransactionConfiguration());

            //Partners
            modelBuilder.ApplyConfiguration(new ApiClientConfiguration());

            ////Ensure all dates are saved as UTC and read as UTC:
            ////https://github.com/dotnet/efcore/issues/4711#issuecomment-481215673

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        modelBuilder.Entity(entityType.ClrType)
                         .Property<DateTime>(property.Name)
                         .HasConversion(
                          v => v.ToUniversalTime(),
                          v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        modelBuilder.Entity(entityType.ClrType)
                         .Property<DateTime?>(property.Name)
                         .HasConversion(
                          v => v.HasValue ? v.Value.ToUniversalTime() : v,
                          v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);
                    }
                }
            }
        }
    }
}
